using ControleDeBar.Dominio.Compartilhado;
using ControleDeBar.Dominio.Compartilhado.Identity;

namespace ControleDeBar.Dominio.Modulos.ModuloGarcom;

public class Garcom : EntidadeBase<Garcom>, IEntidadeDoUsuario
{
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
        Telefone = NormalizarTelefone(telefone);
        Cpf = NormalizarCpf(cpf);
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
        else if (!EhTelefoneValido(Telefone))
            erros.Add("O campo \"Telefone\" deve conter um formato válido.");

        if (string.IsNullOrWhiteSpace(Cpf))
            erros.Add("O campo \"CPF\" deve ser preenchido.");
        else if (!EhCpfValido(Cpf))
            erros.Add("O campo \"CPF\" deve conter 11 dígitos.");

        return erros;
    }

    public override void Atualizar(Garcom entidadeAtualizada)
    {
        Nome = entidadeAtualizada.Nome;
        Telefone = NormalizarTelefone(entidadeAtualizada.Telefone);
        Cpf = NormalizarCpf(entidadeAtualizada.Cpf);
    }

    private static string NormalizarTelefone(string telefone)
    {
        return ExtrairDigitos(telefone);
    }

    private static string NormalizarCpf(string cpf)
    {
        return ExtrairDigitos(cpf);
    }

    private static string ExtrairDigitos(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return string.Empty;

        return new string(valor.Where(char.IsDigit).ToArray());
    }

    private static bool EhTelefoneValido(string telefone)
    {
        if (telefone.Any(char.IsLetter))
            return false;

        if (telefone.Any(c => !char.IsDigit(c) && !EhPontuacaoTelefone(c)))
            return false;

        string digitos = ExtrairDigitos(telefone);

        if (digitos.Length is not (10 or 11))
            return false;

        if (digitos[0] == '0')
            return false;

        if (digitos.Length == 11 && digitos[2] != '9')
            return false;

        return true;
    }

    private static bool EhCpfValido(string cpf)
    {
        if (cpf.Any(char.IsLetter))
            return false;

        if (cpf.Any(c => !char.IsDigit(c) && c is not ('.' or '-')))
            return false;

        return ExtrairDigitos(cpf).Length == 11;
    }

    private static bool EhPontuacaoTelefone(char caractere)
    {
        return caractere is ' ' or '(' or ')' or '-' or '.';
    }
}
