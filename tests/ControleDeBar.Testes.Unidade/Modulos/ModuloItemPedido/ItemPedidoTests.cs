using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloProduto;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloItemPedido;

[TestClass]
public sealed class ItemPedidoTests
{
    [TestMethod]
    public void Validar_AdicionarItem_ContaEmAberto()
    {
        Conta conta = new(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        Produto produto = new("Cerveja", 20.00m);
        int quantidade = 2;

        ItemPedido itemPedido = new(conta.Id, produto.Id, quantidade, produto.Valor);

        conta.AdicionarItem(itemPedido);

        List<string> erros = itemPedido.Validar();
        decimal valorEsperado = produto.Valor * quantidade;

        Assert.HasCount(0, erros);
        Assert.AreEqual(SituacaoConta.Aberta, conta.Situacao);
        Assert.AreEqual(conta.Id, itemPedido.ContaId);
        Assert.AreEqual(produto.Id, itemPedido.ProdutoId);
        Assert.AreEqual(quantidade, itemPedido.Quantidade);
        Assert.AreEqual(produto.Valor, itemPedido.ValorUnitario);
        Assert.AreEqual(valorEsperado, itemPedido.Valor);
        Assert.HasCount(1, conta.Itens);
        Assert.AreEqual(valorEsperado, conta.ValorTotal);
    }

    [TestMethod]
    public void Validar_AdicionarItem_ContaFechada()
    {
        Conta conta = new(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        Produto produto = new("Cerveja", 20.00m);
        ItemPedido itemExistente = new(conta.Id, produto.Id, 1, produto.Valor);
        conta.AdicionarItem(itemExistente);

        decimal totalAntes = conta.ValorTotal;
        int quantidadeItensAntes = conta.Itens.Count;

        conta.Fechar("Ana", "Mesa 01");

        ItemPedido novoItem = new(conta.Id, produto.Id, 2, produto.Valor);
        conta.AdicionarItem(novoItem);

        List<string> erros = novoItem.Validar();

        Assert.HasCount(0, erros);
        Assert.AreEqual(SituacaoConta.Fechada, conta.Situacao);
        Assert.HasCount(quantidadeItensAntes, conta.Itens);
        Assert.AreEqual(totalAntes, conta.ValorTotal);
        Assert.IsFalse(conta.Itens.Contains(novoItem));
    }

    [TestMethod]
    public void Validar_AlterarQuantidade_ContaEmAberto()
    {
        Conta conta = new(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        Produto produto = new("Cerveja", 20.00m);
        ItemPedido itemPedido = new(conta.Id, produto.Id, 2, produto.Valor);
        conta.AdicionarItem(itemPedido);

        int novaQuantidade = 5;
        itemPedido.AlterarQuantidade(novaQuantidade);
        conta.RecalcularValorTotal();

        List<string> erros = itemPedido.Validar();
        decimal valorEsperado = produto.Valor * novaQuantidade;

        Assert.HasCount(0, erros);
        Assert.AreEqual(SituacaoConta.Aberta, conta.Situacao);
        Assert.AreEqual(novaQuantidade, itemPedido.Quantidade);
        Assert.AreEqual(valorEsperado, itemPedido.Valor);
        Assert.AreEqual(valorEsperado, conta.ValorTotal);
    }

    [TestMethod]
    public void Validar_AlterarQuantidade_ComQuantidadeIgualAZero()
    {
        Conta conta = new(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        Produto produto = new("Cerveja", 20.00m);
        ItemPedido itemPedido = new(conta.Id, produto.Id, 2, produto.Valor);
        conta.AdicionarItem(itemPedido);

        int quantidadeAntes = itemPedido.Quantidade;
        decimal valorAntes = itemPedido.Valor;
        decimal totalAntes = conta.ValorTotal;

        itemPedido.AlterarQuantidade(0);
        conta.RecalcularValorTotal();

        Assert.AreEqual(SituacaoConta.Aberta, conta.Situacao);
        Assert.AreEqual(quantidadeAntes, itemPedido.Quantidade);
        Assert.AreEqual(valorAntes, itemPedido.Valor);
        Assert.AreEqual(totalAntes, conta.ValorTotal);
    }

    [TestMethod]
    public void Validar_AlterarQuantidade_ComQuantidadeNegativa()
    {
        Conta conta = new(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        Produto produto = new("Cerveja", 20.00m);
        ItemPedido itemPedido = new(conta.Id, produto.Id, 2, produto.Valor);
        conta.AdicionarItem(itemPedido);

        int quantidadeAntes = itemPedido.Quantidade;
        decimal valorAntes = itemPedido.Valor;
        decimal totalAntes = conta.ValorTotal;

        itemPedido.AlterarQuantidade(-1);
        conta.RecalcularValorTotal();

        Assert.AreEqual(SituacaoConta.Aberta, conta.Situacao);
        Assert.AreEqual(quantidadeAntes, itemPedido.Quantidade);
        Assert.AreEqual(valorAntes, itemPedido.Valor);
        Assert.AreEqual(totalAntes, conta.ValorTotal);
    }

    [TestMethod]
    public void Validar_AlterarQuantidade_ContaFechada()
    {
        Conta conta = new(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        Produto produto = new("Cerveja", 20.00m);
        ItemPedido itemPedido = new(conta.Id, produto.Id, 2, produto.Valor);
        conta.AdicionarItem(itemPedido);

        int quantidadeAntes = itemPedido.Quantidade;
        decimal valorAntes = itemPedido.Valor;
        decimal totalAntes = conta.ValorTotal;

        conta.Fechar("Ana", "Mesa 01");

        itemPedido.AlterarQuantidade(5);
        conta.RecalcularValorTotal();

        Assert.AreEqual(SituacaoConta.Fechada, conta.Situacao);
        Assert.AreEqual(quantidadeAntes, itemPedido.Quantidade);
        Assert.AreEqual(valorAntes, itemPedido.Valor);
        Assert.AreEqual(totalAntes, conta.ValorTotal);
    }

    [TestMethod]
    public void Validar_RemoverItem_ContaEmAberto()
    {
        Conta conta = new(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        Produto produto = new("Cerveja", 20.00m);
        ItemPedido itemRemovido = new(conta.Id, produto.Id, 2, produto.Valor);
        ItemPedido itemRestante = new(conta.Id, produto.Id, 1, produto.Valor);
        conta.AdicionarItem(itemRemovido);
        conta.AdicionarItem(itemRestante);

        conta.RemoverItem(itemRemovido);

        Assert.AreEqual(SituacaoConta.Aberta, conta.Situacao);
        Assert.HasCount(1, conta.Itens);
        Assert.IsFalse(conta.Itens.Contains(itemRemovido));
        Assert.AreEqual(itemRestante, conta.Itens.Single());
        Assert.AreEqual(itemRestante.Valor, conta.ValorTotal);
    }

    [TestMethod]
    public void Validar_RemoverItem_ContaFechada()
    {
        Conta conta = new(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        Produto produto = new("Cerveja", 20.00m);
        ItemPedido itemPedido = new(conta.Id, produto.Id, 2, produto.Valor);
        conta.AdicionarItem(itemPedido);

        int quantidadeItensAntes = conta.Itens.Count;
        decimal totalAntes = conta.ValorTotal;

        conta.Fechar("Ana", "Mesa 01");
        conta.RemoverItem(itemPedido);

        Assert.AreEqual(SituacaoConta.Fechada, conta.Situacao);
        Assert.HasCount(quantidadeItensAntes, conta.Itens);
        Assert.IsTrue(conta.Itens.Contains(itemPedido));
        Assert.AreEqual(totalAntes, conta.ValorTotal);
    }

    [TestMethod]
    public void Validar_AdicionarItem_SemInformarConta_DeveRetornarErro()
    {
        Produto produto = new("Cerveja", 20.00m);

        ItemPedido itemPedido = new(Guid.Empty, produto.Id, 2, produto.Valor);

        List<string> erros = itemPedido.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Conta\" deve ser preenchido.", erros.First());
    }

    [TestMethod]
    public void Validar_AdicionarItem_SemInformarProduto_DeveRetornarErro()
    {
        ItemPedido itemPedido = new(Guid.CreateVersion7(), Guid.Empty, 2, 20.00m);

        List<string> erros = itemPedido.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Produto\" deve ser preenchido.", erros.First());
    }

    [TestMethod]
    public void Validar_AdicionarItem_SemInformarQuantidade_DeveRetornarErro()
    {
        Produto produto = new("Cerveja", 20.00m);

        ItemPedido itemPedido = new(Guid.CreateVersion7(), produto.Id, default, produto.Valor);

        List<string> erros = itemPedido.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Quantidade\" deve ser maior que zero.", erros.First());
    }

    [TestMethod]
    public void Validar_AdicionarItem_ComQuantidadeIgualAZero_DeveRetornarErro()
    {
        Produto produto = new("Cerveja", 20.00m);

        ItemPedido itemPedido = new(Guid.CreateVersion7(), produto.Id, 0, produto.Valor);

        List<string> erros = itemPedido.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Quantidade\" deve ser maior que zero.", erros.First());
    }

    [TestMethod]
    public void Validar_AdicionarItem_ComQuantidadeNegativa_DeveRetornarErro()
    {
        Produto produto = new("Cerveja", 20.00m);

        ItemPedido itemPedido = new(Guid.CreateVersion7(), produto.Id, -1, produto.Valor);

        List<string> erros = itemPedido.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual("O campo \"Quantidade\" deve ser maior que zero.", erros.First());
    }
}
