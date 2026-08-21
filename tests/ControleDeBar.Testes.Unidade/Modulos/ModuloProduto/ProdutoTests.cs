using ControleDeBar.Dominio.Modulos.ModuloProduto;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloProduto;

[TestClass]
public sealed class ProdutoTests
{
    [TestMethod]
    public void Validar_DeveCadastrar_ComDadosValidos()
    {
        Produto produto = new("Cerveja", 20.00m);

        List<string> erros = produto.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_ComNomeVazio_DeveRetornarErro()
    {
        Produto produto = new(string.Empty, 30.00m);

        List<string> erros = produto.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Nome\" deve ser preenchido.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_ComValorVazio_DeveRetornarErro()
    {
        Produto produto = new("Cerveja", 0);

        List<string> erros = produto.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Valor\" deve ser maior que zero.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_ComNomeCurto_DeveRetornarErro()
    {
        Produto produto = new(new string('A', 1), 20.00m);

        List<string> erros = produto.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Nome\" deve conter no mínimo 3 caracteres.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_ComNomeTamanhoLimite()
    {
        Produto produto = new(new string('A', 3), 30.00m);

        List<string> erros = produto.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_ComNomeLonga_DeveRetornarErro()
    {
        Produto produto = new(new string('A', 101), 30.00m);

        List<string> erros = produto.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Nome\" deve conter no máximo 100 caracteres.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_ComNomeTamanhoMaximo()
    {
        Produto produto = new(new string('A', 100), 30.00m);

        List<string> erros = produto.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_ComPrecoNegativo_DeveRetornarErro()
    {
        Produto produto = new("Cerveja", -10.00m);

        List<string> erros = produto.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Valor\" deve ser maior que zero.",
            erros.First()
        );
    }

    [TestMethod]
    public void Atualizar_DeveAtualizar_DadosValidos()
    {
        Produto produto = new Produto(
            "Cerveja",
            20.00m
        );

        Produto produtoAtualizado = new Produto(
            "Cerveja",
            30.00m
        );

        produto.Atualizar(produtoAtualizado);

        Assert.AreEqual("Cerveja", produto.Nome);
        Assert.AreEqual(30.00m, produto.Valor);
    }

}