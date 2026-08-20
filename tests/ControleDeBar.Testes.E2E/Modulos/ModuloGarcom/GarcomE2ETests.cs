using ControleDeBar.Testes.E2E.Compartilhado;
using ControleDeBar.Testes.E2E.Modulos.ModuloConta;
using ControleDeBar.Testes.E2E.Modulos.ModuloMesa;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloGarcom;

[TestClass]
public sealed class GarcomE2ETests : E2ETestsBase
{
    [TestMethod]
    public async Task DeveCadastrar_Garcom_ComDadosValidos()
    {
        await RegistrarEEntrarAsync(
            "garcom.cadastro@teste.local",
            "Senha123!"
        );

        GarcomFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Teste", "(00) 00000-0000", "111.111.111-11");
        await formPage.ConfirmarAsync();

        GarcomListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await Expect(listarPage.NomeDoGarcom("Teste"))
            .ToBeVisibleAsync();

        await Expect(listarPage.TelefoneDoGarcom("Teste"))
            .ToHaveTextAsync("(00) 00000-0000");

        await Expect(listarPage.CpfDoGarcom("Teste"))
            .ToHaveTextAsync("111.111.111-11");
    }

    [TestMethod]
    public async Task Cadastrar_ComCpfDuplicado_ImpedeCadastro_RetornaMensagemDeErro()
    {
        await RegistrarEEntrarAsync(
            "garcom.cpf.duplicado@teste.local",
            "Senha123!"
        );

        GarcomFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Teste", "(00) 00000-0000", "111.111.111-11");
        await formPage.ConfirmarAsync();

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Outro", "(11) 11111-1111", "111.111.111-11");
        await formPage.ConfirmarAsync();

        await Expect(
            formPage.MensagemErro(
                "Já existe um garçom com este CPF."
            )
        ).ToBeVisibleAsync();

        GarcomListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await Expect(listarPage.NomeDoGarcom("Teste"))
            .ToBeVisibleAsync();

        await Expect(listarPage.NomeDoGarcom("Outro"))
            .Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task Cadastrar_ComTelefoneDuplicado_ImpedeCadastro_RetornaMensagemDeErro()
    {
        await RegistrarEEntrarAsync(
            "garcom.telefone.duplicado@teste.local",
            "Senha123!"
        );

        GarcomFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Teste", "(00) 00000-0000", "111.111.111-11");
        await formPage.ConfirmarAsync();

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Outro", "(00) 00000-0000", "222.222.222-22");
        await formPage.ConfirmarAsync();

        await Expect(
            formPage.MensagemErro(
                "Já existe um garçom com este telefone."
            )
        ).ToBeVisibleAsync();

        GarcomListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await Expect(listarPage.NomeDoGarcom("Teste"))
            .ToBeVisibleAsync();

        await Expect(listarPage.NomeDoGarcom("Outro"))
            .Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task DeveEditar_Garcom_ComDadosValidos()
    {
        await RegistrarEEntrarAsync(
            "garcom.editar@teste.local",
            "Senha123!"
        );

        GarcomFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Teste", "(00) 00000-0000", "111.111.111-11");
        await formPage.ConfirmarAsync();

        GarcomListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await listarPage.EditarAsync("Teste");

        await Page.WaitForURLAsync("**/Garcom/Editar**");
        await Expect(formPage.Nome).ToBeVisibleAsync();

        await formPage.PreencherAsync("Teste Editado", "(22) 22222-2222", "222.222.222-22");
        await formPage.ConfirmarAsync();

        await Expect(listarPage.NomeDoGarcom("Teste Editado"))
            .ToBeVisibleAsync();

        await Expect(listarPage.TelefoneDoGarcom("Teste Editado"))
            .ToHaveTextAsync("(22) 22222-2222");

        await Expect(listarPage.CpfDoGarcom("Teste Editado"))
            .ToHaveTextAsync("222.222.222-22");
    }

    [TestMethod]
    public async Task DeveImpedirEditar_Garcom_ComCpfDuplicado()
    {
        await RegistrarEEntrarAsync(
            "garcom.editar.cpf.duplicado@teste.local",
            "Senha123!"
        );

        GarcomFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Teste", "(00) 00000-0000", "111.111.111-11");
        await formPage.ConfirmarAsync();

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Outro", "(11) 11111-1111", "222.222.222-22");
        await formPage.ConfirmarAsync();

        GarcomListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await listarPage.EditarAsync("Teste");

        await Page.WaitForURLAsync("**/Garcom/Editar**");
        await Expect(formPage.Nome).ToBeVisibleAsync();

        await formPage.PreencherAsync("Teste", "(00) 00000-0000", "222.222.222-22");
        await formPage.ConfirmarAsync();

        await Expect(
            formPage.MensagemErro(
                "Já existe um garçom com este CPF."
            )
        ).ToBeVisibleAsync();

        await listarPage.IrParaAsync();

        await Expect(listarPage.CpfDoGarcom("Teste"))
            .ToHaveTextAsync("111.111.111-11");

        await Expect(listarPage.CpfDoGarcom("Outro"))
            .ToHaveTextAsync("222.222.222-22");
    }

    [TestMethod]
    public async Task DeveImpedirEditar_Garcom_ComTelefoneDuplicado()
    {
        await RegistrarEEntrarAsync(
            "garcom.editar.telefone.duplicado@teste.local",
            "Senha123!"
        );

        GarcomFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Teste", "(00) 00000-0000", "111.111.111-11");
        await formPage.ConfirmarAsync();

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Outro", "(11) 11111-1111", "222.222.222-22");
        await formPage.ConfirmarAsync();

        GarcomListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await listarPage.EditarAsync("Teste");

        await Page.WaitForURLAsync("**/Garcom/Editar**");
        await Expect(formPage.Nome).ToBeVisibleAsync();

        await formPage.PreencherAsync("Teste", "(11) 11111-1111", "111.111.111-11");
        await formPage.ConfirmarAsync();

        await Expect(
            formPage.MensagemErro(
                "Já existe um garçom com este telefone."
            )
        ).ToBeVisibleAsync();

        await listarPage.IrParaAsync();

        await Expect(listarPage.TelefoneDoGarcom("Teste"))
            .ToHaveTextAsync("(00) 00000-0000");

        await Expect(listarPage.TelefoneDoGarcom("Outro"))
            .ToHaveTextAsync("(11) 11111-1111");
    }

    [TestMethod]
    public async Task DeveListar_TodosOsGarcons()
    {
        await RegistrarEEntrarAsync(
            "garcom.listar@teste.local",
            "Senha123!"
        );

        GarcomFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Teste", "(00) 00000-0000", "111.111.111-11");
        await formPage.ConfirmarAsync();

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Outro", "(11) 11111-1111", "222.222.222-22");
        await formPage.ConfirmarAsync();

        GarcomListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await Expect(listarPage.NomeDoGarcom("Teste"))
            .ToBeVisibleAsync();

        await Expect(listarPage.TelefoneDoGarcom("Teste"))
            .ToHaveTextAsync("(00) 00000-0000");

        await Expect(listarPage.CpfDoGarcom("Teste"))
            .ToHaveTextAsync("111.111.111-11");

        await Expect(listarPage.NomeDoGarcom("Outro"))
            .ToBeVisibleAsync();

        await Expect(listarPage.TelefoneDoGarcom("Outro"))
            .ToHaveTextAsync("(11) 11111-1111");

        await Expect(listarPage.CpfDoGarcom("Outro"))
            .ToHaveTextAsync("222.222.222-22");
    }

    [TestMethod]
    public async Task DeveExcluir_Garcom_SemContaEmAbertoVinculada()
    {
        await RegistrarEEntrarAsync(
            "garcom.excluir.livre@teste.local",
            "Senha123!"
        );

        GarcomFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Teste", "(00) 00000-0000", "111.111.111-11");
        await formPage.ConfirmarAsync();

        GarcomListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await listarPage.ExcluirAsync("Teste");

        GarcomExcluirPage excluirPage = new(Page);

        await Expect(excluirPage.MensagemConfirmacao)
            .ToBeVisibleAsync();

        await excluirPage.ConfirmarAsync();

        await Expect(Page)
            .ToHaveURLAsync($"{UrlBase}/Garcom/Listar");

        await Expect(listarPage.NomeDoGarcom("Teste"))
            .Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task DeveImpedirExcluir_Garcom_VinculadoAContaEmAberto()
    {
        await RegistrarEEntrarAsync(
            "garcom.excluir.conta.aberta@teste.local",
            "Senha123!"
        );

        GarcomFormPage formPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync();
        await formPage.PreencherAsync("Teste", "(00) 00000-0000", "111.111.111-11");
        await formPage.ConfirmarAsync();

        MesaFormPage mesaFormPage = new(Page, UrlBase);

        await mesaFormPage.IrParaCadastroAsync();
        await mesaFormPage.PreencherAsync("Mesa01", "4");
        await mesaFormPage.ConfirmarAsync();

        ContaFormPage contaFormPage = new(Page, UrlBase);

        await contaFormPage.IrParaCadastroAsync();
        await contaFormPage.PreencherAsync("Cliente", "Mesa01", "Teste");
        await contaFormPage.ConfirmarAsync();

        GarcomListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await listarPage.ExcluirAsync("Teste");

        GarcomExcluirPage excluirPage = new(Page);

        await Expect(excluirPage.MensagemConfirmacao)
            .ToBeVisibleAsync();

        await excluirPage.ConfirmarAsync();

        await Expect(Page)
            .ToHaveURLAsync($"{UrlBase}/Garcom/Listar");

        await Expect(listarPage.MensagemErro)
            .ToHaveTextAsync("Não é possível excluir este garçom porque ele está vinculado a uma conta em aberto.");

        await Expect(listarPage.NomeDoGarcom("Teste"))
            .ToBeVisibleAsync();
    }
}
