using ControleDeBar.Dominio.Compartilhado;
using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;

namespace ControleDeBar.Dominio.Modulos.ModuloConta;

public class Conta : EntidadeBase<Conta>, IEntidadeDoUsuario
{
    public string NomeCliente { get; set; } = string.Empty;
    public Guid? MesaId { get; set; }
    public Mesa? Mesa { get; set; }
    public string IdentificacaoMesa { get; set; } = string.Empty;
    public Guid? GarcomId { get; set; }
    public Garcom? Garcom { get; set; }
    public string NomeGarcom { get; set; } = string.Empty;
    public DateTime DataAbertura { get; private set; }
    public SituacaoConta Situacao { get; private set; }
    public decimal ValorTotal { get; private set; }
    public List<ItemPedido> Itens { get; set; } = [];
    public Guid UserId { get; set; }

    public bool EstaAberta => Situacao == SituacaoConta.Aberta;
    public bool EstaFechada => Situacao == SituacaoConta.Fechada;

    public Conta()
    {
    }

    public Conta(string nomeCliente, Guid mesaId, string identificacaoMesa, Guid garcomId, string nomeGarcom) : this()
    {
        NomeCliente = nomeCliente;
        MesaId = mesaId;
        IdentificacaoMesa = identificacaoMesa;
        GarcomId = garcomId;
        NomeGarcom = nomeGarcom;
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

        if (!MesaId.HasValue || MesaId.Value == Guid.Empty)
            erros.Add("O campo \"Mesa\" deve ser preenchido.");
        else if (string.IsNullOrWhiteSpace(IdentificacaoMesa))
            erros.Add("O campo \"Mesa\" deve ser preenchido.");

        if (!GarcomId.HasValue || GarcomId.Value == Guid.Empty)
            erros.Add("O campo \"Garçom\" deve ser preenchido.");
        else if (string.IsNullOrWhiteSpace(NomeGarcom))
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
        IdentificacaoMesa = entidadeAtualizada.IdentificacaoMesa;
        GarcomId = entidadeAtualizada.GarcomId;
        NomeGarcom = entidadeAtualizada.NomeGarcom;
    }

    public void Fechar(string nomeGarcom, string identificacaoMesa)
    {
        NomeGarcom = nomeGarcom;
        IdentificacaoMesa = identificacaoMesa;
        Situacao = SituacaoConta.Fechada;
    }

    public void AdicionarItem(ItemPedido item)
    {
        if (EstaFechada)
            return;

        item.AtribuirConta(this);
        Itens.Add(item);
        RecalcularValorTotal();
    }

    public void RemoverItem(ItemPedido item)
    {
        if (EstaFechada)
            return;

        Itens.Remove(item);
        RecalcularValorTotal();
    }

    public void AtualizarValorTotal(decimal valorTotal)
    {
        ValorTotal = valorTotal;
    }

    public void RecalcularValorTotal()
    {
        ValorTotal = Itens.Sum(i => i.Valor);
    }

    public void RecalcularValorTotal(IEnumerable<decimal> valoresDosItens)
    {
        ValorTotal = valoresDosItens.Sum();
    }

    public DateTime? ObterDataUltimoPedido()
    {
        if (EstaFechada || Itens.Count == 0)
            return null;

        return Itens.Max(i => i.DataAdicao);
    }
}
