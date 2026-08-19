using System.Text.RegularExpressions;
using ControleDeBar.Dominio.Compartilhado;
using ControleDeBar.Dominio.Compartilhado.Identity;

namespace ControleDeBar.Dominio.Modulos.ModuloGarcom;

public class Garcom : EntidadeBase<Garcom>, IEntidadeDoUsuario
{
    private const string PadraoTelefone = @"^\(\d{2}\) \d{4,5}-\d{4}$";
    private const string PadraoCpf = @"^\d{3}\.\d{3}\.\d{3}-\d{2}$";

    public string Nome { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public Guid UserId { get; set; }

    public Garcom()
    {
    }

    public Garcom(string nome, string telefone, string cpf) : this()
    {
        Nome = nome;
        Telefone = telefone;
        Cpf = cpf;
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(Nome))
            erros.Add("O campo \"Nome\" deve ser preenchido.");
        else if (Nome.Length < 3)
            erros.Add("O campo \"Nome\" deve conter no mínimo 3 caracteres.");
        else if (Nome.Length > 100)
            erros.Add("O campo \"Nome\" deve conter no máximo 100 caracteres.");

        if (string.IsNullOrWhiteSpace(Telefone))
            erros.Add("O campo \"Telefone\" deve ser preenchido.");
        else if (!Regex.IsMatch(Telefone, PadraoTelefone))
            erros.Add("O campo \"Telefone\" deve estar no formato (XX) XXXX-XXXX ou (XX) XXXXX-XXXX.");

        if (string.IsNullOrWhiteSpace(Cpf))
            erros.Add("O campo \"Cpf\" deve ser preenchido.");
        else if (ExtrairDigitos(Cpf).Length != 11)
            erros.Add("O campo \"Cpf\" deve conter 11 dígitos.");
        else if (!Regex.IsMatch(Cpf, PadraoCpf))
            erros.Add("O campo \"Cpf\" deve estar no formato XXX.XXX.XXX-XX.");

        return erros;
    }

    public override void Atualizar(Garcom entidadeAtualizada)
    {
        Nome = entidadeAtualizada.Nome;
        Telefone = entidadeAtualizada.Telefone;
        Cpf = entidadeAtualizada.Cpf;
    }

    private static string ExtrairDigitos(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return string.Empty;

        return new string(valor.Where(char.IsDigit).ToArray());
    }
}
