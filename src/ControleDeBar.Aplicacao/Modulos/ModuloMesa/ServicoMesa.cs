using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using FluentResults;

namespace ControleDeBar.Aplicacao.Modulos.ModuloMesa;

public class ServicoMesa : ServicoBase<Mesa>
{
    private readonly IRepositorioMesa repositorioMesa;
    private readonly IRepositorioConta repositorioConta;

    public ServicoMesa(IRepositorioMesa repositorioMesa, IRepositorioConta repositorioConta)
    {
        this.repositorioMesa = repositorioMesa;
        this.repositorioConta = repositorioConta;
    }

    public Result Cadastrar(CadastrarMesaDto dto)
    {
        if (ExisteMesaComMesmoNumero(dto.Identificacao))
            return Falha(nameof(dto.Identificacao), "Já existe uma mesa com este número.");

        Mesa novaMesa = new Mesa(dto.Identificacao, dto.QuantidadeLugar);

        if (dto.StatusMesa != StatusMesa.Livre)
            return Falha(nameof(dto.StatusMesa), "Uma mesa cadastrada deve iniciar com o status Livre.");

        novaMesa = new Mesa(dto.Identificacao, dto.QuantidadeLugar);

        Result resultadoValidacao = ValidarEntidade(novaMesa);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioMesa.Cadastrar(novaMesa);

        return Result.Ok();
    }

    public Result Editar(EditarMesaDto dto)
    {
        if (ExisteMesaComMesmoNumero(dto.Identificacao, dto.Id))
            return Falha(nameof(dto.Identificacao), "Já existe uma mesa com este número.");

        Mesa mesaAtualizada = new Mesa(dto.Identificacao, dto.QuantidadeLugar);

        if (dto.StatusMesa == StatusMesa.Ocupada)
            mesaAtualizada.MarcarComoOcupada();

        Result resultadoValidacao = ValidarEntidade(mesaAtualizada);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        bool conseguiuEditar = repositorioMesa.Editar(dto.Id, mesaAtualizada);

        if (!conseguiuEditar)
            return Falha(string.Empty, "Mesa não encontrada.");

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Mesa? mesa = repositorioMesa.SelecionarPorId(id);

        if (mesa == null)
            return Falha(string.Empty, "Mesa não encontrada.");

        if (repositorioConta.ExisteContaAbertaPorMesa(mesa.Id))
            return Falha(string.Empty, "Não é possível excluir uma mesa vinculada a uma conta em aberto");

        bool conseguiuExcluir = repositorioMesa.Excluir(id);

        if (!conseguiuExcluir)
            return Falha(string.Empty, "Não foi possível excluir a mesa.");

        return Result.Ok();
    }

    public List<ListarMesasDto> SelecionarTodos()
    {
        return repositorioMesa
            .SelecionarTodos()
            .Select(m => new ListarMesasDto(
                m.Id,
                m.Identificacao,
                m.QuantidadeLugar,
                m.StatusMesa
            ))
            .ToList();
    }

    public Result<DetalhesMesaDto> SelecionarPorId(Guid id)
    {
        Mesa? mesa = repositorioMesa.SelecionarPorId(id);

        if (mesa == null)
            return Result.Fail("Mesa não encontrada.");

        return Result.Ok(new DetalhesMesaDto(
            mesa.Id,
            mesa.Identificacao,
            mesa.QuantidadeLugar,
            mesa.StatusMesa
        ));
    }

    private bool ExisteMesaComMesmoNumero(string identificacao, Guid? idIgnorado = null)
    {
        string identificacaoNormalizada = NormalizarIdentificacao(identificacao);

        return repositorioMesa
            .SelecionarTodos()
            .Any(m =>
                m.Id != idIgnorado &&
                NormalizarIdentificacao(m.Identificacao) == identificacaoNormalizada
            );
    }

    private static string NormalizarIdentificacao(string identificacao)
    {
        return identificacao.Trim().ToLowerInvariant();
    }
}
