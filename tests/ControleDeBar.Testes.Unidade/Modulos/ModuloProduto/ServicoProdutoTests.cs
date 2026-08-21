using ControleDeBar.Aplicacao.Modulos.ModuloProduto;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using FluentResults;
using Moq;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloProduto;

[TestClass]
public sealed class ServicoProdutoTests
{
    [TestMethod]
    public void Cadastrar_ComTodosCampos_PersisteProduto()
    {
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        repositorioProduto.Setup(r => r.SelecionarTodos()).Returns([]);

        Produto? produtoCadastrado = null;

        repositorioProduto
            .Setup(r => r.Cadastrar(It.IsAny<Produto>()))
            .Callback<Produto>(
                produto => produtoCadastrado = produto
            );

        ServicoProduto servicoProduto = new ServicoProduto(
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoProduto.Cadastrar(new CadastrarProdutoDto(
            "Teste",
            30.00m
        ));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(produtoCadastrado);
        Assert.AreEqual("Teste", produtoCadastrado.Nome);
        Assert.AreEqual(30.00m, produtoCadastrado.Valor);

        repositorioProduto.Verify(r => r.Cadastrar(It.IsAny<Produto>()), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_ComNomeVazio_RetornaErro()
    {
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        repositorioProduto.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoProduto servicoProduto = new(
            repositorioProduto.Object,
            repositorioItemPedido.Object

        );

        Result resultado = servicoProduto.Cadastrar(new CadastrarProdutoDto(
            string.Empty,
            30.00m
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("O campo \"Nome\" deve ser preenchido.", resultado.Errors.First().Message);

        repositorioProduto.Verify(r => r.Cadastrar(It.IsAny<Produto>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_ComValorVazio_RetornaErro()
    {
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        repositorioProduto.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoProduto servicoProduto = new(
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoProduto.Cadastrar(new CadastrarProdutoDto(
            "Cerveja",
            0
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("O campo \"Valor\" deve ser maior que zero.", resultado.Errors.First().Message);

        repositorioProduto.Verify(r => r.Cadastrar(It.IsAny<Produto>()), Times.Never);
    }

    [TestMethod]
    public void Atualizar_ComDadosValidos_PersisteProduto()
    {
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Produto produtoExistente = new Produto(
            "Teste1",
            40.00m
        );

        List<Produto> produto = new() { produtoExistente };

        repositorioProduto.Setup(r => r.SelecionarTodos()).Returns(() => produto);
        repositorioProduto
            .Setup(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Produto>()))
            .Callback<Guid, Produto>((id, produtoAtualizado) =>
            {
                produtoAtualizado.Id = id;
                int index = produto.FindIndex(g => g.Id == id);
                if (index >= 0)
                    produto[index].Atualizar(produtoAtualizado);
            })
            .Returns<Guid, Produto>((id, produtoAtualizado) => produto.Any(g => g.Id == id));

        ServicoProduto servicoProduto = new ServicoProduto(
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoProduto.Editar(new EditarProdutoDto(
            produtoExistente.Id,
            "Teste2",
            40.00m
        ));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioProduto.Verify(r => r.Editar(produtoExistente.Id, It.IsAny<Produto>()), Times.Once);

        List<ListarProdutosDto> produtosListados = servicoProduto.SelecionarTodos();

        Assert.HasCount(1, produtosListados);
        Assert.AreEqual("Teste2", produtosListados[0].Nome);
        Assert.AreEqual(40.00m, produtosListados[0].Valor);
    }

    [TestMethod]
    public void SelecionarPorId_RetornaProduto()
    {
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Produto produtoExistente = new Produto(
            "Teste",
            30.00m
        );

        repositorioProduto
            .Setup(r => r.SelecionarPorId(produtoExistente.Id))
            .Returns(produtoExistente);

        ServicoProduto servicoProduto = new ServicoProduto(
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result<DetalhesProdutoDto> resultado = servicoProduto.SelecionarPorId(produtoExistente.Id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(resultado.Value);
        Assert.AreEqual(produtoExistente.Id, resultado.Value.Id);
        Assert.AreEqual("Teste", resultado.Value.Nome);
        Assert.AreEqual(30.00m, resultado.Value.Valor);
    }

    [TestMethod]
    public void SelecionarTodos_RetornaProdutosCadastrados()
    {
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        List<Produto> produtos = new()
    {
        new Produto(
            "Teste1",
            30.00m
        ),
        new Produto(
            "Teste2",
            50.00m
        )
    };

        repositorioProduto.Setup(r => r.SelecionarTodos()).Returns(() => produtos);

        ServicoProduto servicoProduto = new ServicoProduto(
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        List<ListarProdutosDto> produtosListados = servicoProduto.SelecionarTodos();

        Assert.HasCount(2, produtosListados);
        Assert.AreEqual("Teste1", produtosListados[0].Nome);
        Assert.AreEqual(30.00m, produtosListados[0].Valor);

        Assert.AreEqual("Teste2", produtosListados[1].Nome);
        Assert.AreEqual(50.00m, produtosListados[1].Valor);
    }

    [TestMethod]
    public void Excluir_SemItemPedidosVinculadas_ExcluiProduto()
    {
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Produto produto = new Produto(
            "Teste2",
            50.00m
        );

        repositorioProduto
            .Setup(r => r.SelecionarPorId(produto.Id))
            .Returns(produto);

        repositorioItemPedido
            .Setup(r => r.ExistePorProduto(produto.Id))
            .Returns(false);

        ServicoProduto servicoProduto = new(
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoProduto.Excluir(produto.Id);

        Assert.IsTrue(resultado.IsSuccess);

        repositorioProduto.Verify(
            r => r.Excluir(produto.Id),
            Times.Once
        );
    }

    [TestMethod]
    public void Excluir_ComItemPedidosVinculados_RetornaErro()
    {
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Produto produto = new Produto("Teste", 30.00m);

        repositorioProduto
            .Setup(r => r.SelecionarPorId(produto.Id))
            .Returns(produto);

        repositorioItemPedido
            .Setup(r => r.ExistePorProduto(produto.Id))
            .Returns(true);

        ServicoProduto servicoProduto = new(
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoProduto.Excluir(produto.Id);

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Não é possível excluir este produto porque ele está vinculado a um ou mais itens de pedido.",
            resultado.Errors.First().Message
        );

        repositorioProduto.Verify(
            r => r.Excluir(It.IsAny<Guid>()),
            Times.Never
        );
    }


}