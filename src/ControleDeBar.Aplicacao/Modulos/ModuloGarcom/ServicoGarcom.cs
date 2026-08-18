using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using FluentResults;

namespace ControleDeBar.Aplicacao.Modulos.ModuloGarcom;

public class ServicoGarcom : ServicoBase<Garcom>
{
    private readonly IRepositorioGarcom repositorioGarcom;
    private readonly IRepositorioConta repositorioConta;

    public ServicoGarcom(IRepositorioGarcom repositorioGarcom, IRepositorioConta repositorioConta)
    {
        this.repositorioGarcom = repositorioGarcom;
        this.repositorioConta = repositorioConta;
    }

    public Result Cadastrar(CadastrarGarcomDto dto)
    {
        if (ExisteGarcomComMesmoCpf(dto.Cpf))
            return Falha(nameof(dto.Cpf), "Já existe um garçom com este CPF.");

        if (ExisteGarcomComMesmoTelefone(dto.Telefone))
            return Falha(nameof(dto.Telefone), "Já existe um garçom com este telefone.");

        Garcom novoGarcom = new Garcom(dto.Nome, dto.Telefone, dto.Cpf);

        Result resultadoValidacao = ValidarEntidade(novoGarcom);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioGarcom.Cadastrar(novoGarcom);

        return Result.Ok();
    }

    public Result Editar(EditarGarcomDto dto)
    {
        if (ExisteGarcomComMesmoCpf(dto.Cpf, dto.Id))
            return Falha(nameof(dto.Cpf), "Já existe um garçom com este CPF.");

        if (ExisteGarcomComMesmoTelefone(dto.Telefone, dto.Id))
            return Falha(nameof(dto.Telefone), "Já existe um garçom com este telefone.");

        Garcom garcomAtualizado = new Garcom(dto.Nome, dto.Telefone, dto.Cpf);

        Result resultadoValidacao = ValidarEntidade(garcomAtualizado);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        bool conseguiuEditar = repositorioGarcom.Editar(dto.Id, garcomAtualizado);

        if (!conseguiuEditar)
            return Falha(string.Empty, "Garçom não encontrado.");

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Garcom? garcom = repositorioGarcom.SelecionarPorId(id);

        if (garcom == null)
            return Falha(string.Empty, "Garçom não encontrado.");

        if (PossuiVinculoComContaEmAberto(garcom.Id))
            return Falha(string.Empty, "Não é possível excluir este garçom porque ele está vinculado a uma conta em aberto.");

        repositorioGarcom.Excluir(id);

        return Result.Ok();
    }

    public List<ListarGarconsDto> SelecionarTodos()
    {
        return repositorioGarcom
            .SelecionarTodos()
            .Select(g => new ListarGarconsDto(
                g.Id,
                g.Nome,
                g.Telefone,
                g.Cpf
            ))
            .ToList();
    }

    public Result<DetalhesGarcomDto> SelecionarPorId(Guid id)
    {
        Garcom? garcom = repositorioGarcom.SelecionarPorId(id);

        if (garcom == null)
            return Result.Fail("Garçom não encontrado.");

        return Result.Ok(new DetalhesGarcomDto(
            garcom.Id,
            garcom.Nome,
            garcom.Telefone,
            garcom.Cpf
        ));
    }

    private bool PossuiVinculoComContaEmAberto(Guid garcomId)
    {
        return repositorioConta.ExisteContaAbertaPorGarcom(garcomId);
    }

    private bool ExisteGarcomComMesmoCpf(string cpf, Guid? idIgnorado = null)
    {
        string cpfNormalizado = NormalizarDigitos(cpf);

        return repositorioGarcom
            .SelecionarTodos()
            .Any(g =>
                g.Id != idIgnorado &&
                NormalizarDigitos(g.Cpf) == cpfNormalizado
            );
    }

    private bool ExisteGarcomComMesmoTelefone(string telefone, Guid? idIgnorado = null)
    {
        string telefoneNormalizado = NormalizarDigitos(telefone);

        return repositorioGarcom
            .SelecionarTodos()
            .Any(g =>
                g.Id != idIgnorado &&
                NormalizarDigitos(g.Telefone) == telefoneNormalizado
            );
    }

    private static string NormalizarDigitos(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return string.Empty;

        return new string(valor.Where(char.IsDigit).ToArray());
    }
}
