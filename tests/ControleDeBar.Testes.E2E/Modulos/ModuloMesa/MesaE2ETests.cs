using ControleDeBar.Testes.E2E.Compartilhado;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloMesa;

[TestClass]
public sealed class MesaE2ETests : E2ETestsBase
{
    [TestMethod]
    public async Task DeveCadastrar_Mesa_ComDadosValidos()
    {
        await RegistrarEEntrarAsync(
            "mesa.cadastro@teste.local",
            "Senha123!"
        );

        MesaFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Mesa01", "4");
        await formPage.ConfirmarAsync();

        MesaListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await Expect(listarPage.NomeDaMesa("Mesa01"))
            .ToBeVisibleAsync();

        await Expect(listarPage.QuantidadeDeLugares("Mesa01"))
            .ToHaveTextAsync("4");

        await Expect(listarPage.StatusDaMesa("Mesa01"))
            .ToHaveTextAsync("Livre");
    }

    [TestMethod]
    public async Task DeveEditar_Mesa_ComDadosValidos()
    {
        await RegistrarEEntrarAsync(
            "mesa.editar@teste.local",
            "Senha123!"
        );

        MesaFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Mesa01", "4");
        await formPage.ConfirmarAsync();

        MesaListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await listarPage.EditarAsync("Mesa01");

        await Page.WaitForURLAsync("**/Mesa/Editar**");
        await Expect(formPage.Identificacao).ToBeVisibleAsync();

        await formPage.PreencherAsync("Mesa01-Editada", "6");
        await formPage.ConfirmarAsync();

        await Expect(listarPage.NomeDaMesa("Mesa01-Editada"))
            .ToBeVisibleAsync();

        await Expect(listarPage.QuantidadeDeLugares("Mesa01-Editada"))
            .ToHaveTextAsync("6");
    }

    [TestMethod]
    public async Task DeveExcluir_Mesa_Livre_SemContaVinculada()
    {
        await RegistrarEEntrarAsync(
            "mesa.excluir.livre@teste.local",
            "Senha123!"
        );

        MesaFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Mesa01", "4");
        await formPage.ConfirmarAsync();

        MesaListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await listarPage.ExcluirAsync("Mesa01");

        MesaExcluirPage excluirPage = new(Page);

        await Expect(excluirPage.MensagemConfirmacao)
            .ToBeVisibleAsync();

        await excluirPage.ConfirmarAsync();

        await Expect(Page)
            .ToHaveURLAsync($"{UrlBase}/Mesa/Listar");

        await Expect(listarPage.NomeDaMesa("Mesa01"))
            .Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task DeveListar_TodasAsMesas()
    {
        await RegistrarEEntrarAsync(
            "mesa.listar@teste.local",
            "Senha123!"
        );

        MesaFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Mesa01", "4");
        await formPage.ConfirmarAsync();

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Mesa02", "6");
        await formPage.ConfirmarAsync();

        MesaListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await Expect(listarPage.NomeDaMesa("Mesa01"))
            .ToBeVisibleAsync();

        await Expect(listarPage.NomeDaMesa("Mesa02"))
            .ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task NaoDeveCadastrar_Mesa_ComNumeroDuplicado()
    {
        await RegistrarEEntrarAsync(
            "mesa.duplicada@teste.local",
            "Senha123!"
        );

        MesaFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Mesa01", "4");
        await formPage.ConfirmarAsync();

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Mesa01", "6");
        await formPage.ConfirmarAsync();

        await Expect(
            formPage.MensagemErro(
                "Já existe uma mesa com este número."
            )
        ).ToBeVisibleAsync();
    }
}