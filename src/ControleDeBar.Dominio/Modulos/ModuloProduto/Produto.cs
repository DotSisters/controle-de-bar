using ControleDeBar.Dominio.Compartilhado;
using ControleDeBar.Dominio.Compartilhado.Identity;

namespace ControleDeBar.Dominio.Modulos.ModuloProduto;

public class Produto : EntidadeBase<Produto>, IEntidadeDoUsuario
{
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public Guid UserId { get; set; }

    public Produto()
    {
    }

    public Produto(string nome, decimal valor) : this()
    {
        Nome = nome;
        Valor = valor;
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

        if (Valor <= 0)
            erros.Add("O campo \"Valor\" deve ser maior que zero.");

        return erros;
    }

    public override void Atualizar(Produto entidadeAtualizada)
    {
        Nome = entidadeAtualizada.Nome;
        Valor = entidadeAtualizada.Valor;
    }

}