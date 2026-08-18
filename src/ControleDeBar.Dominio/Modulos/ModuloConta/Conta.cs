using ControleDeBar.Dominio.Compartilhado;
using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;

namespace ControleDeBar.Dominio.Modulos.ModuloConta;

public class Conta : EntidadeBase<Conta>, IEntidadeDoUsuario
{
    public string NomeCliente { get; set; } = string.Empty;
    public Guid MesaId { get; set; }
    public Mesa? Mesa { get; set; }
    public Guid GarcomId { get; set; }
    public Garcom? Garcom { get; set; }
    public DateTime DataAbertura { get; private set; }
    public SituacaoConta Situacao { get; private set; }
    public decimal ValorTotal { get; private set; }
    public Guid UserId { get; set; }

    public bool EstaAberta => Situacao == SituacaoConta.Aberta;
    public bool EstaFechada => Situacao == SituacaoConta.Fechada;

    public Conta()
    {
    }

    public Conta(string nomeCliente, Guid mesaId, Guid garcomId) : this()
    {
        NomeCliente = nomeCliente;
        MesaId = mesaId;
        GarcomId = garcomId;
        DataAbertura = DateTime.Now;
        Situacao = SituacaoConta.Aberta;
        ValorTotal = 0m;
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(NomeCliente))
            erros.Add("O campo \"Nome do Cliente\" deve ser preenchido.");
        else if (NomeCliente.Length < 3)
            erros.Add("O campo \"Nome do Cliente\" deve conter no mínimo 3 caracteres.");
        else if (NomeCliente.Length > 100)
            erros.Add("O campo \"Nome do Cliente\" deve conter no máximo 100 caracteres.");

        if (MesaId == Guid.Empty)
            erros.Add("O campo \"Mesa\" deve ser preenchido.");

        if (GarcomId == Guid.Empty)
            erros.Add("O campo \"Garçom\" deve ser preenchido.");

        if (DataAbertura == default)
            erros.Add("O campo \"Data de Abertura\" deve ser preenchido.");

        if (!Enum.IsDefined(Situacao))
            erros.Add("O campo \"Situação\" deve ser preenchido.");

        return erros;
    }

    public override void Atualizar(Conta entidadeAtualizada)
    {
        NomeCliente = entidadeAtualizada.NomeCliente;
        MesaId = entidadeAtualizada.MesaId;
        GarcomId = entidadeAtualizada.GarcomId;
    }

    public void Fechar()
    {
        Situacao = SituacaoConta.Fechada;
    }

    public void AtualizarValorTotal(decimal valorTotal)
    {
        ValorTotal = valorTotal;
    }

    public void RecalcularValorTotal(IEnumerable<decimal> valoresDosPedidos)
    {
        ValorTotal = valoresDosPedidos.Sum();
    }
}
