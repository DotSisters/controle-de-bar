using ControleDeBar.Dominio.Compartilhado;
using ControleDeBar.Dominio.Compartilhado.Identity;

namespace ControleDeBar.Dominio.Modulos.ModuloMesa;

public class Mesa : EntidadeBase<Mesa>, IEntidadeDoUsuario
{
    public string Identificacao { get; set; } = string.Empty;
    public int QuantidadeLugar { get; set; }
    public StatusMesa StatusMesa { get; private set; }
    public Guid UserId { get; set; }

    public Mesa()
    {
    }

    public Mesa(string identificacao, int quantidadeLugar) : this()
    {
        Identificacao = identificacao;
        QuantidadeLugar = quantidadeLugar;
        StatusMesa = StatusMesa.Livre;
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(Identificacao))
            erros.Add("O campo \"Identificação\" deve ser preenchido.");
        else if (Identificacao.Length < 2)
            erros.Add("O campo \"Identificação\" deve conter no mínimo 2 caracteres.");
        else if (Identificacao.Length > 20)
            erros.Add("O campo \"Identificação\" deve conter no máximo 20 caracteres.");

        if (!Enum.IsDefined(StatusMesa))
            erros.Add("O campo \"Status Mesa\" deve ser preenchido.");

        if (QuantidadeLugar <= 0)
            erros.Add("O campo \"Quantidade de Lugares\" deve ser maior que zero.");
        else if (QuantidadeLugar > 20)
            erros.Add("O campo \"Quantidade de Lugares\" deve ser no máximo 20.");

        if (StatusMesa != StatusMesa.Livre)
            erros.Add("Uma mesa cadastrada deve iniciar com o status Livre.");

        return erros;
    }

    public override void Atualizar(Mesa entidadeAtualizada)
    {
        Identificacao = entidadeAtualizada.Identificacao;
        QuantidadeLugar = entidadeAtualizada.QuantidadeLugar;
        StatusMesa = entidadeAtualizada.StatusMesa;
    }

    public void MarcarComoOcupada()
    {
        StatusMesa = StatusMesa.Ocupada;
    }

    public void MarcarComoLivre()
    {
        StatusMesa = StatusMesa.Livre;
    }
}