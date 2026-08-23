using ControleDeBar.Dominio.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloProduto;

namespace ControleDeBar.Dominio.Modulos.ModuloConta;

public class ItemPedido : EntidadeBase<ItemPedido>
{
    public Guid ContaId { get; private set; }
    public Conta? Conta { get; private set; }
    public Guid ProdutoId { get; private set; }
    public Produto? Produto { get; private set; }
    public int Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public decimal Valor { get; private set; }
    public DateTime DataAdicao { get; private set; }

    public ItemPedido()
    {
    }

    public ItemPedido(Guid contaId, Guid produtoId, int quantidade, decimal valorUnitario) : this()
    {
        ContaId = contaId;
        ProdutoId = produtoId;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
        DataAdicao = DateTime.Now;
        CalcularValor();
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (ContaId == Guid.Empty)
            erros.Add("O campo \"Conta\" deve ser preenchido.");

        if (ProdutoId == Guid.Empty)
            erros.Add("O campo \"Produto\" deve ser preenchido.");

        if (Quantidade <= 0)
            erros.Add("O campo \"Quantidade\" deve ser maior que zero.");

        if (ValorUnitario < 0)
            erros.Add("O valor unitário do item de pedido é inválido.");

        if (DataAdicao == default)
            erros.Add("O campo \"Data de Adição\" deve ser preenchido.");

        return erros;
    }

    public override void Atualizar(ItemPedido entidadeAtualizada)
    {
        Quantidade = entidadeAtualizada.Quantidade;
        CalcularValor();
    }

    public void AlterarQuantidade(int quantidade)
    {
        if (quantidade <= 0)
            return;

        if (Conta is { EstaFechada: true })
            return;

        Quantidade = quantidade;
        CalcularValor();
    }

    internal void AtribuirConta(Conta conta)
    {
        Conta = conta;
    }

    private void CalcularValor()
    {
        Valor = ValorUnitario * Quantidade;
    }
}
