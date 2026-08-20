using ControleDeBar.Testes.E2E.Compartilhado;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloProduto;

[TestClass]
public sealed class ProdutoE2ETests : E2ETestsBase
{
    [TestMethod]
    public async Task DeveCadastrar_Produto_ComDadosValidos()
    {
        await RegistrarEEntrarAsync("produto.cadastro@teste.local", "Senha123!");

        ProdutoFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Produto01", 30.00m);
        await formPage.ConfirmarAsync();

        ProdutoListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await Expect(listarPage.NomeDoProduto("Produto01"))
            .ToBeVisibleAsync();

        await Expect(listarPage.Valor("Produto01"))
            .ToContainTextAsync("R$ 30,00");
    }

    [TestMethod]
    public async Task DeveEditar_Produto_ComDadosValidos()
    {
        await RegistrarEEntrarAsync(
            "produto.editar@teste.local",
            "Senha123!"
        );

        ProdutoFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Produto01", 4.00m);
        await formPage.ConfirmarAsync();

        ProdutoListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await listarPage.EditarAsync("Produto01");

        await Page.WaitForURLAsync("**/Produto/Editar**");
        await Expect(formPage.Nome).ToBeVisibleAsync();

        await formPage.PreencherAsync("Produto01-Editada", 60.00m);
        await formPage.ConfirmarAsync();

        await Expect(listarPage.NomeDoProduto("Produto01-Editada"))
            .ToBeVisibleAsync();

        await Expect(listarPage.Valor("Produto01-Editada"))
            .ToContainTextAsync("R$ 60,00");
    }

    [TestMethod]
    public async Task DeveExcluir_Produto_Livre_SemContaVinculada()
    {
        await RegistrarEEntrarAsync(
            "produto.excluir.livre@teste.local",
            "Senha123!"
        );

        ProdutoFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Produto01", 10.00m);
        await formPage.ConfirmarAsync();

        ProdutoListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await listarPage.ExcluirAsync("Produto01");

        ProdutoExcluirPage excluirPage = new(Page);

        await Expect(excluirPage.MensagemConfirmacao)
            .ToBeVisibleAsync();

        await excluirPage.ConfirmarAsync();

        await Expect(Page)
            .ToHaveURLAsync($"{UrlBase}/Produto/Listar");

        await Expect(listarPage.NomeDoProduto("Produto01"))
            .Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task DeveListar_TodasAsProdutos()
    {
        await RegistrarEEntrarAsync(
            "produto.listar@teste.local",
            "Senha123!"
        );

        ProdutoFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Produto01", 7.00m);
        await formPage.ConfirmarAsync();

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Produto02", 10.00m);
        await formPage.ConfirmarAsync();

        ProdutoListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await Expect(listarPage.NomeDoProduto("Produto01"))
            .ToBeVisibleAsync();

        await Expect(listarPage.NomeDoProduto("Produto02"))
            .ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task NaoDeveCadastrar_Produto_ComNomeDuplicado()
    {
        await RegistrarEEntrarAsync(
            "produto.duplicada@teste.local",
            "Senha123!"
        );

        ProdutoFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Produto01", 10.00m);
        await formPage.ConfirmarAsync();

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Produto01", 12.00m);
        await formPage.ConfirmarAsync();

        await Expect(
            formPage.MensagemErro(
                "Já existe um produto com este nome."
            )
        ).ToBeVisibleAsync();
    }
}