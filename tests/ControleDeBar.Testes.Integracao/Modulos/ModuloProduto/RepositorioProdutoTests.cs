using ControleDeBar.Dominio.Modulos.ModuloProduto;
using ControleDeBar.Testes.Integracao.Compartilhado.Orm;
using FizzWare.NBuilder;

namespace ControleDeBar.Testes.Integracao.Modulos.ModuloProduto;

[TestClass]
public class RepositorioProdutoTests : RepositorioBaseTests
{
    [TestMethod]
    public void Cadastrar_ComTodosOsCampos_RegistraProduto()
    {
        // arrange
        Produto produto = Builder<Produto>
            .CreateNew()
            .With(p => p.Nome = "Cerveja")
            .With(p => p.Valor = 20.00m)
            .With(p => p.UserId = Guid.Empty)
            .Build();

        repositorioProduto.Cadastrar(produto);
        dbContext.ChangeTracker.Clear();

        // act
        Produto? produtoSelecionado = repositorioProduto.SelecionarPorId(produto.Id);

        // assert
        Assert.IsNotNull(produtoSelecionado);
        Assert.AreEqual("Cerveja", produtoSelecionado.Nome);
        Assert.AreEqual(20.00m, produtoSelecionado.Valor);
    }

    [TestMethod]
    public void Editar_ComDadosValidos_AtualizaProduto()
    {
        // arrange
        Produto produto = Builder<Produto>
            .CreateNew()
            .With(p => p.Nome = "Refrigerante")
            .With(p => p.Valor = 10.00m)
            .With(p => p.UserId = Guid.Empty)
            .Build();

        repositorioProduto.Cadastrar(produto);
        dbContext.ChangeTracker.Clear();

        // act
        produto.Nome = "Refrigerante Editado";
        produto.Valor = 12.00m;
        repositorioProduto.Editar(produto.Id, produto);

        dbContext.ChangeTracker.Clear();

        Produto? produtoSelecionado = repositorioProduto.SelecionarPorId(produto.Id);

        // assert
        Assert.IsNotNull(produtoSelecionado);
        Assert.AreEqual("Refrigerante Editado", produtoSelecionado.Nome);
        Assert.AreEqual(12.00m, produtoSelecionado.Valor);
    }

    [TestMethod]
    public void Visualizar_ProdutoCadastrado_RetornaProduto()
    {
        // arrange
        Produto produto = Builder<Produto>
            .CreateNew()
            .With(p => p.Nome = "Suco")
            .With(p => p.Valor = 8.00m)
            .With(p => p.UserId = Guid.Empty)
            .Build();

        repositorioProduto.Cadastrar(produto);
        dbContext.ChangeTracker.Clear();

        // act
        Produto? produtoSelecionado = repositorioProduto.SelecionarPorId(produto.Id);

        // assert
        Assert.IsNotNull(produtoSelecionado);
        Assert.AreEqual("Suco", produtoSelecionado.Nome);
    }

    [TestMethod]
    public void Listar_TodosOsProdutos_RetornaTodos()
    {
        // arrange
        var produtos = Builder<Produto>.CreateListOfSize(3)
            .All()
            .With(p => p.UserId = Guid.Empty)
            .Build();

        foreach (var produto in produtos)
            repositorioProduto.Cadastrar(produto);

        dbContext.ChangeTracker.Clear();

        // act
        var produtosSelecionados = repositorioProduto.SelecionarTodos();

        // assert
        Assert.AreEqual(3, produtosSelecionados.Count);
    }

    [TestMethod]
    public void Excluir_ProdutoSemPedidoVinculado_RemoveProduto()
    {
        // arrange
        Produto produto = Builder<Produto>
            .CreateNew()
            .With(p => p.Nome = "Água")
            .With(p => p.Valor = 3.00m)
            .With(p => p.UserId = Guid.Empty)
            .Build();

        repositorioProduto.Cadastrar(produto);
        dbContext.ChangeTracker.Clear();

        // act
        bool conseguiuExcluir = repositorioProduto.Excluir(produto.Id);

        dbContext.ChangeTracker.Clear();

        Produto? produtoSelecionado = repositorioProduto.SelecionarPorId(produto.Id);

        // assert
        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(produtoSelecionado);
    }
}
