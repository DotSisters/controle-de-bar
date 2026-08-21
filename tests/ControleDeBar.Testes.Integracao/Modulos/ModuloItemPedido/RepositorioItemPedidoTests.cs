using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using ControleDeBar.Testes.Integracao.Compartilhado.Orm;
using FizzWare.NBuilder;

namespace ControleDeBar.Testes.Integracao.Modulos.ModuloItemPedido;

[TestClass]
public class RepositorioItemPedidoTests : RepositorioBaseTests
{
    [TestMethod]
    public void Cadastrar_ComQuantidadeEPreco_CalculaValorDoItem()
    {
        // arrange
        Produto produto = Builder<Produto>
            .CreateNew()
            .With(p => p.Nome = "Cerveja")
            .With(p => p.Valor = 7.50m)
            .With(p => p.UserId = Guid.Empty)
            .Build();

        repositorioProduto.Cadastrar(produto);

        Conta conta = new("João Silva", Guid.CreateVersion7(), "Mesa 01", Guid.CreateVersion7(), "Ana");
        repositorioConta.Cadastrar(conta);

        ItemPedido itemPedido = new(conta.Id, produto.Id, 3, produto.Valor);

        repositorioItemPedido.Cadastrar(itemPedido);
        dbContext.ChangeTracker.Clear();

        // act
        ItemPedido? itemSelecionado = repositorioItemPedido.SelecionarPorId(itemPedido.Id);

        // assert
        Assert.IsNotNull(itemSelecionado);
        Assert.AreEqual(produto.Id, itemSelecionado.ProdutoId);
        Assert.AreEqual(3, itemSelecionado.Quantidade);
        Assert.AreEqual(7.50m, itemSelecionado.ValorUnitario);
        Assert.AreEqual(22.50m, itemSelecionado.Valor);
    }

    [TestMethod]
    public void SelecionarPorConta_ContaAbertaComItens_RetornaTodosItens()
    {
        // arrange
        Produto cerveja = Builder<Produto>
            .CreateNew()
            .With(p => p.Nome = "Cerveja")
            .With(p => p.Valor = 7.50m)
            .With(p => p.UserId = Guid.Empty)
            .Build();

        Produto suco = Builder<Produto>
            .CreateNew()
            .With(p => p.Nome = "Suco de Laranja")
            .With(p => p.Valor = 5.00m)
            .With(p => p.UserId = Guid.Empty)
            .Build();

        Produto agua = Builder<Produto>
            .CreateNew()
            .With(p => p.Nome = "Água")
            .With(p => p.Valor = 3.00m)
            .With(p => p.UserId = Guid.Empty)
            .Build();

        repositorioProduto.Cadastrar(cerveja);
        repositorioProduto.Cadastrar(suco);
        repositorioProduto.Cadastrar(agua);

        Conta conta = new("João Silva", Guid.CreateVersion7(), "Mesa 01", Guid.CreateVersion7(), "Ana");
        repositorioConta.Cadastrar(conta);

        ItemPedido itemCerveja = new(conta.Id, cerveja.Id, 3, cerveja.Valor);
        ItemPedido itemSuco = new(conta.Id, suco.Id, 2, suco.Valor);
        ItemPedido itemAgua = new(conta.Id, agua.Id, 1, agua.Valor);

        repositorioItemPedido.Cadastrar(itemCerveja);
        repositorioItemPedido.Cadastrar(itemSuco);
        repositorioItemPedido.Cadastrar(itemAgua);
        dbContext.ChangeTracker.Clear();

        // act
        List<ItemPedido> itensSelecionados = repositorioItemPedido.SelecionarPorConta(conta.Id);

        // assert
        Assert.AreEqual(3, itensSelecionados.Count);

        ItemPedido cervejaSelecionada = itensSelecionados.Single(i => i.Id == itemCerveja.Id);
        Assert.AreEqual("Cerveja", cervejaSelecionada.Produto?.Nome);
        Assert.AreEqual(3, cervejaSelecionada.Quantidade);
        Assert.AreEqual(7.50m, cervejaSelecionada.ValorUnitario);
        Assert.AreEqual(22.50m, cervejaSelecionada.Valor);

        ItemPedido sucoSelecionado = itensSelecionados.Single(i => i.Id == itemSuco.Id);
        Assert.AreEqual("Suco de Laranja", sucoSelecionado.Produto?.Nome);
        Assert.AreEqual(2, sucoSelecionado.Quantidade);
        Assert.AreEqual(5.00m, sucoSelecionado.ValorUnitario);
        Assert.AreEqual(10.00m, sucoSelecionado.Valor);

        ItemPedido aguaSelecionada = itensSelecionados.Single(i => i.Id == itemAgua.Id);
        Assert.AreEqual("Água", aguaSelecionada.Produto?.Nome);
        Assert.AreEqual(1, aguaSelecionada.Quantidade);
        Assert.AreEqual(3.00m, aguaSelecionada.ValorUnitario);
        Assert.AreEqual(3.00m, aguaSelecionada.Valor);
    }

    [TestMethod]
    public void Editar_ContaAberta_AtualizaQuantidadeValorETotal()
    {
        // arrange
        Produto produto = Builder<Produto>
            .CreateNew()
            .With(p => p.Nome = "Cerveja")
            .With(p => p.Valor = 20.00m)
            .With(p => p.UserId = Guid.Empty)
            .Build();

        repositorioProduto.Cadastrar(produto);

        Conta conta = new("João Silva", Guid.CreateVersion7(), "Mesa 01", Guid.CreateVersion7(), "Ana");
        repositorioConta.Cadastrar(conta);

        ItemPedido itemPedido = new(conta.Id, produto.Id, 2, produto.Valor);
        repositorioItemPedido.Cadastrar(itemPedido);
        dbContext.ChangeTracker.Clear();

        int novaQuantidade = 5;
        decimal valorEsperado = produto.Valor * novaQuantidade;
        ItemPedido itemAtualizado = new(conta.Id, produto.Id, novaQuantidade, produto.Valor);

        // act
        repositorioItemPedido.Editar(itemPedido.Id, itemAtualizado);
        dbContext.ChangeTracker.Clear();

        ItemPedido? itemSelecionado = repositorioItemPedido.SelecionarPorId(itemPedido.Id);
        Conta? contaSelecionada = repositorioConta.SelecionarPorId(conta.Id);
        decimal totalDaConta = repositorioItemPedido
            .SelecionarPorConta(conta.Id)
            .Sum(i => i.Valor);

        // assert
        Assert.IsNotNull(itemSelecionado);
        Assert.IsNotNull(contaSelecionada);
        Assert.AreEqual(SituacaoConta.Aberta, contaSelecionada.Situacao);
        Assert.AreEqual(novaQuantidade, itemSelecionado.Quantidade);
        Assert.AreEqual(valorEsperado, itemSelecionado.Valor);
        Assert.AreEqual(valorEsperado, totalDaConta);
    }

    [TestMethod]
    public void Excluir_ContaAberta_RemoveItemERecalculaTotal()
    {
        // arrange
        Produto produto = Builder<Produto>
            .CreateNew()
            .With(p => p.Nome = "Cerveja")
            .With(p => p.Valor = 20.00m)
            .With(p => p.UserId = Guid.Empty)
            .Build();

        repositorioProduto.Cadastrar(produto);

        Conta conta = new("João Silva", Guid.CreateVersion7(), "Mesa 01", Guid.CreateVersion7(), "Ana");
        repositorioConta.Cadastrar(conta);

        ItemPedido itemRemovido = new(conta.Id, produto.Id, 2, produto.Valor);
        ItemPedido itemRestante = new(conta.Id, produto.Id, 1, produto.Valor);

        repositorioItemPedido.Cadastrar(itemRemovido);
        repositorioItemPedido.Cadastrar(itemRestante);
        dbContext.ChangeTracker.Clear();

        // act
        bool conseguiuExcluir = repositorioItemPedido.Excluir(itemRemovido.Id);
        dbContext.ChangeTracker.Clear();

        ItemPedido? itemExcluido = repositorioItemPedido.SelecionarPorId(itemRemovido.Id);
        ItemPedido? itemMantido = repositorioItemPedido.SelecionarPorId(itemRestante.Id);
        Conta? contaSelecionada = repositorioConta.SelecionarPorId(conta.Id);
        List<ItemPedido> itensDaConta = repositorioItemPedido.SelecionarPorConta(conta.Id);
        decimal totalDaConta = itensDaConta.Sum(i => i.Valor);

        // assert
        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(itemExcluido);
        Assert.IsNotNull(itemMantido);
        Assert.IsNotNull(contaSelecionada);
        Assert.AreEqual(SituacaoConta.Aberta, contaSelecionada.Situacao);
        Assert.AreEqual(1, itensDaConta.Count);
        Assert.AreEqual(itemRestante.Id, itensDaConta.Single().Id);
        Assert.AreEqual(itemRestante.Valor, totalDaConta);
    }
}
