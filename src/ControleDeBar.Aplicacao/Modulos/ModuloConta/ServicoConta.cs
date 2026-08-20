using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using FluentResults;

namespace ControleDeBar.Aplicacao.Modulos.ModuloConta;

public class ServicoConta : ServicoBase<Conta>
{
    private readonly IRepositorioConta repositorioConta;
    private readonly IRepositorioMesa repositorioMesa;
    private readonly IRepositorioGarcom repositorioGarcom;
    private readonly IRepositorioProduto repositorioProduto;
    private readonly IRepositorioItemPedido repositorioItemPedido;

    public ServicoConta(
        IRepositorioConta repositorioConta,
        IRepositorioMesa repositorioMesa,
        IRepositorioGarcom repositorioGarcom,
        IRepositorioProduto repositorioProduto,
        IRepositorioItemPedido repositorioItemPedido
    )
    {
        this.repositorioConta = repositorioConta;
        this.repositorioMesa = repositorioMesa;
        this.repositorioGarcom = repositorioGarcom;
        this.repositorioProduto = repositorioProduto;
        this.repositorioItemPedido = repositorioItemPedido;
    }

    public Result Cadastrar(CadastrarContaDto dto)
    {
        Mesa? mesa = repositorioMesa.SelecionarPorId(dto.MesaId);

        if (mesa == null)
            return Falha(nameof(dto.MesaId), "Mesa não encontrada.");

        if (repositorioConta.ExisteContaAbertaPorMesa(mesa.Id))
            return Falha(nameof(dto.MesaId), "Não é possível abrir uma conta para esta mesa porque ela já possui uma conta em aberto.");

        Garcom? garcom = repositorioGarcom.SelecionarPorId(dto.GarcomId);

        if (garcom == null)
            return Falha(nameof(dto.GarcomId), "Garçom não encontrado.");

        Conta novaConta = new Conta(dto.NomeCliente, mesa.Id, mesa.Identificacao, garcom.Id, garcom.Nome);

        Result resultadoValidacao = ValidarEntidade(novaConta);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        mesa.MarcarComoOcupada();
        repositorioConta.Cadastrar(novaConta);

        return Result.Ok();
    }

    public Result Editar(EditarContaDto dto)
    {
        Conta? conta = repositorioConta.SelecionarPorId(dto.Id);

        if (conta == null)
            return Falha(string.Empty, "Conta não encontrada.");

        if (conta.EstaFechada)
            return Falha(string.Empty, "Não é possível editar uma conta fechada.");

        Mesa? mesa = repositorioMesa.SelecionarPorId(dto.MesaId);

        if (mesa == null)
            return Falha(nameof(dto.MesaId), "Mesa não encontrada.");

        Garcom? garcom = repositorioGarcom.SelecionarPorId(dto.GarcomId);

        if (garcom == null)
            return Falha(nameof(dto.GarcomId), "Garçom não encontrado.");

        Conta contaAtualizada = new Conta(dto.NomeCliente, mesa.Id, mesa.Identificacao, garcom.Id, garcom.Nome);

        Result resultadoValidacao = ValidarEntidade(contaAtualizada);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        if (conta.MesaId != mesa.Id)
        {
            if (repositorioConta.ExisteContaAbertaPorMesa(mesa.Id))
                return Falha(nameof(dto.MesaId), "Não é possível vincular esta conta a esta mesa porque ela já possui uma conta em aberto.");

            Mesa? mesaAnterior = repositorioMesa.SelecionarPorId(conta.MesaId!.Value);
            mesaAnterior?.MarcarComoLivre();
            mesa.MarcarComoOcupada();
        }

        bool conseguiuEditar = repositorioConta.Editar(dto.Id, contaAtualizada);

        if (!conseguiuEditar)
            return Falha(string.Empty, "Conta não encontrada.");

        return Result.Ok();
    }

    public Result Fechar(Guid id)
    {
        Conta? conta = repositorioConta.SelecionarPorId(id);

        if (conta == null)
            return Falha(string.Empty, "Conta não encontrada.");

        if (conta.EstaFechada)
            return Falha(string.Empty, "Não é possível fechar uma conta que já está fechada.");

        Mesa? mesa = repositorioMesa.SelecionarPorId(conta.MesaId!.Value);

        conta.Fechar(
            conta.Garcom?.Nome ?? repositorioGarcom.SelecionarPorId(conta.GarcomId.Value)!.Nome,
            conta.Mesa?.Identificacao ?? mesa!.Identificacao
        );

        mesa?.MarcarComoLivre();

        bool conseguiuEditar = repositorioConta.Editar(id, conta);

        if (!conseguiuEditar)
            return Falha(string.Empty, "Conta não encontrada.");

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Conta? conta = repositorioConta.SelecionarPorId(id);

        if (conta == null)
            return Falha(string.Empty, "Conta não encontrada.");

        if (conta.EstaAberta)
        {
            Mesa? mesa = repositorioMesa.SelecionarPorId(conta.MesaId!.Value);
            mesa?.MarcarComoLivre();
        }

        repositorioConta.Excluir(id);

        return Result.Ok();
    }

    public Result AdicionarPedido(AdicionarPedidoContaDto dto)
    {
        Conta? conta = repositorioConta.SelecionarPorId(dto.ContaId);

        if (conta == null)
            return Falha(string.Empty, "Conta não encontrada.");

        if (conta.EstaFechada)
            return Falha(string.Empty, "Não é possível adicionar itens de pedido a uma conta fechada.");

        if (dto.ProdutoId == Guid.Empty)
            return Falha(nameof(dto.ProdutoId), "O campo \"Produto\" deve ser preenchido.");

        Produto? produto = repositorioProduto.SelecionarPorId(dto.ProdutoId);

        if (produto == null)
            return Falha(nameof(dto.ProdutoId), "Produto não encontrado.");

        if (dto.Quantidade <= 0)
            return Falha(nameof(dto.Quantidade), "O campo \"Quantidade\" deve ser maior que zero.");

        ItemPedido novoItem = new ItemPedido(conta.Id, produto.Id, dto.Quantidade, produto.Valor);

        Result resultadoValidacao = ValidarItemPedido(novoItem);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        conta.AdicionarItem(novoItem);
        repositorioItemPedido.Cadastrar(novoItem);

        return Result.Ok();
    }

    public Result AlterarQuantidadeItemPedido(AlterarQuantidadeItemPedidoDto dto)
    {
        Conta? conta = repositorioConta.SelecionarPorId(dto.ContaId);

        if (conta == null)
            return Falha(string.Empty, "Conta não encontrada.");

        if (conta.EstaFechada)
            return Falha(string.Empty, "Não é possível alterar a quantidade de um item de uma conta fechada.");

        ItemPedido? item = conta.Itens.FirstOrDefault(i => i.Id == dto.ItemPedidoId);

        if (item == null)
            return Falha(string.Empty, "Item de pedido não encontrado.");

        if (dto.Quantidade <= 0)
            return Falha(nameof(dto.Quantidade), "O campo \"Quantidade\" deve ser maior que zero.");

        item.AlterarQuantidade(dto.Quantidade);

        Result resultadoValidacao = ValidarItemPedido(item);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        conta.RecalcularValorTotal();

        bool conseguiuEditar = repositorioItemPedido.Editar(item.Id, item);

        if (!conseguiuEditar)
            return Falha(string.Empty, "Item de pedido não encontrado.");

        return Result.Ok();
    }

    public Result RemoverItemPedido(RemoverItemPedidoDto dto)
    {
        Conta? conta = repositorioConta.SelecionarPorId(dto.ContaId);

        if (conta == null)
            return Falha(string.Empty, "Conta não encontrada.");

        if (conta.EstaFechada)
            return Falha(string.Empty, "Não é possível remover itens de uma conta fechada.");

        ItemPedido? item = conta.Itens.FirstOrDefault(i => i.Id == dto.ItemPedidoId);

        if (item == null)
            return Falha(string.Empty, "Item de pedido não encontrado.");

        conta.RecalcularValorTotal(conta.Itens.Where(i => i.Id != item.Id).Select(i => i.Valor));

        bool conseguiuExcluir = repositorioItemPedido.Excluir(item.Id);

        if (!conseguiuExcluir)
            return Falha(string.Empty, "Item de pedido não encontrado.");

        return Result.Ok();
    }

    public List<ListarContasDto> SelecionarTodos()
    {
        return repositorioConta
            .SelecionarTodos()
            .Select(MapearParaListarDto)
            .ToList();
    }

    public Result<DetalhesContaDto> SelecionarPorId(Guid id)
    {
        Conta? conta = repositorioConta.SelecionarPorId(id);

        if (conta == null)
            return Result.Fail("Conta não encontrada.");

        return Result.Ok(MapearParaDetalhesDto(conta));
    }

    private static Result ValidarItemPedido(ItemPedido item)
    {
        List<string> erros = item.Validar();

        if (erros.Count == 0)
            return Result.Ok();

        return Falha(string.Empty, erros.First());
    }

    private static ListarContasDto MapearParaListarDto(Conta conta)
    {
        return new ListarContasDto(
            conta.Id,
            conta.NomeCliente,
            conta.IdentificacaoMesa,
            conta.NomeGarcom,
            conta.DataAbertura,
            conta.Situacao,
            conta.ValorTotal
        );
    }

    private static DetalhesContaDto MapearParaDetalhesDto(Conta conta)
    {
        return new DetalhesContaDto(
            conta.Id,
            conta.NomeCliente,
            conta.MesaId,
            conta.IdentificacaoMesa,
            conta.GarcomId,
            conta.NomeGarcom,
            conta.DataAbertura,
            conta.Situacao,
            conta.ValorTotal,
            conta.Itens
                .Select(i => new ItemPedidoContaDto(
                    i.Id,
                    i.Produto?.Nome ?? string.Empty,
                    i.Quantidade,
                    i.ValorUnitario,
                    i.Valor
                ))
                .ToList()
        );
    }
}
