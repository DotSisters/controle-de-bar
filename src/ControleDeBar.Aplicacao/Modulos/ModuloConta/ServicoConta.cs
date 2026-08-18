using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using FluentResults;

namespace ControleDeBar.Aplicacao.Modulos.ModuloConta;

public class ServicoConta : ServicoBase<Conta>
{
    private readonly IRepositorioConta repositorioConta;
    private readonly IRepositorioMesa repositorioMesa;
    private readonly IRepositorioGarcom repositorioGarcom;

    public ServicoConta(
        IRepositorioConta repositorioConta,
        IRepositorioMesa repositorioMesa,
        IRepositorioGarcom repositorioGarcom
    )
    {
        this.repositorioConta = repositorioConta;
        this.repositorioMesa = repositorioMesa;
        this.repositorioGarcom = repositorioGarcom;
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

        Conta novaConta = new Conta(dto.NomeCliente, mesa.Id, garcom.Id);

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

        Conta contaAtualizada = new Conta(dto.NomeCliente, mesa.Id, garcom.Id);

        Result resultadoValidacao = ValidarEntidade(contaAtualizada);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        if (conta.MesaId != mesa.Id)
        {
            if (repositorioConta.ExisteContaAbertaPorMesa(mesa.Id))
                return Falha(nameof(dto.MesaId), "Não é possível vincular esta conta a esta mesa porque ela já possui uma conta em aberto.");

            Mesa? mesaAnterior = repositorioMesa.SelecionarPorId(conta.MesaId);
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

        conta.Fechar();

        Mesa? mesa = repositorioMesa.SelecionarPorId(conta.MesaId);
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
            Mesa? mesa = repositorioMesa.SelecionarPorId(conta.MesaId);
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
            return Falha(string.Empty, "Não é possível adicionar pedidos a uma conta fechada.");

        return Falha(
            string.Empty,
            "A inclusão de pedidos estará disponível após a implementação do módulo de Pedidos."
        );
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

    private static ListarContasDto MapearParaListarDto(Conta conta)
    {
        return new ListarContasDto(
            conta.Id,
            conta.NomeCliente,
            conta.Mesa?.Identificacao ?? string.Empty,
            conta.Garcom?.Nome ?? string.Empty,
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
            conta.Mesa?.Identificacao ?? string.Empty,
            conta.GarcomId,
            conta.Garcom?.Nome ?? string.Empty,
            conta.DataAbertura,
            conta.Situacao,
            conta.ValorTotal,
            []
        );
    }
}
