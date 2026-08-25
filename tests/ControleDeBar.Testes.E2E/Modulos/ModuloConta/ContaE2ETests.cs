using System.Text.RegularExpressions;
using ControleDeBar.Testes.E2E.Compartilhado;
using ControleDeBar.Testes.E2E.Modulos.ModuloGarcom;
using ControleDeBar.Testes.E2E.Modulos.ModuloItemPedido;
using ControleDeBar.Testes.E2E.Modulos.ModuloMesa;
using ControleDeBar.Testes.E2E.Modulos.ModuloProduto;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloConta;

[TestClass]
public sealed class ContaE2ETests : E2ETestsBase
{
    [TestMethod]
    public async Task DeveAbrir_Conta_ComDadosValidos()
    {
        await RegistrarEEntrarAsync("conta.abrir@teste.local", "Senha123!");

        MesaFormPage mesaForm = new(Page, UrlBase);
        await mesaForm.IrParaCadastroAsync();
        await mesaForm.PreencherAsync("Mesa01", "4");
        await mesaForm.ConfirmarAsync();

        GarcomFormPage garcomForm = new(Page, UrlBase);
        await garcomForm.IrParaCadastroAsync();
        await garcomForm.PreencherAsync("João", "(00) 00000-0000", "111.111.111-11");
        await garcomForm.ConfirmarAsync();

        ContaFormPage contaForm = new(Page, UrlBase);
        await contaForm.IrParaCadastroAsync();
        await contaForm.PreencherAsync("Cliente Teste", "Mesa01", "João");
        await contaForm.ConfirmarAsync();

        ContaListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await Expect(listarPage.NomeDoConta("Cliente Teste")).ToBeVisibleAsync();
        await Expect(listarPage.StatusDaConta("Cliente Teste")).ToHaveTextAsync("Aberta");

        MesaListarPage mesaListar = new(Page, UrlBase);
        await mesaListar.IrParaAsync();
        await Expect(mesaListar.StatusDaMesa("Mesa01")).ToHaveTextAsync("Ocupada");
    }

    [TestMethod]
    public async Task NaoDeveAbrir_Conta_EmMesaJaOcupada()
    {
        await RegistrarEEntrarAsync("conta.mesa.ocupada@teste.local", "Senha123!");

        MesaFormPage mesaForm = new(Page, UrlBase);
        await mesaForm.IrParaCadastroAsync();
        await mesaForm.PreencherAsync("Mesa01", "4");
        await mesaForm.ConfirmarAsync();

        GarcomFormPage garcomForm = new(Page, UrlBase);
        await garcomForm.IrParaCadastroAsync();
        await garcomForm.PreencherAsync("João", "(00) 00000-0000", "111.111.111-11");
        await garcomForm.ConfirmarAsync();

        ContaFormPage contaForm = new(Page, UrlBase);
        await contaForm.IrParaCadastroAsync();
        await contaForm.PreencherAsync("Cliente 1", "Mesa01", "João");
        await contaForm.ConfirmarAsync();

        await contaForm.IrParaCadastroAsync();
        await contaForm.PreencherAsync("Cliente 2", "Mesa01", "João");
        await contaForm.ConfirmarAsync();

        await Expect(contaForm.MensagemErro)
            .ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task DeveEditar_Conta_Aberta()
    {
        await RegistrarEEntrarAsync("conta.editar@teste.local", "Senha123!");

        MesaFormPage mesaForm = new(Page, UrlBase);
        await mesaForm.IrParaCadastroAsync();
        await mesaForm.PreencherAsync("Mesa01", "4");
        await mesaForm.ConfirmarAsync();

        GarcomFormPage garcomForm = new(Page, UrlBase);
        await garcomForm.IrParaCadastroAsync();
        await garcomForm.PreencherAsync("João", "(00) 00000-0000", "111.111.111-11");
        await garcomForm.ConfirmarAsync();

        ContaFormPage contaForm = new(Page, UrlBase);
        await contaForm.IrParaCadastroAsync();
        await contaForm.PreencherAsync("Cliente Teste", "Mesa01", "João");
        await contaForm.ConfirmarAsync();

        ContaListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();
        await listarPage.EditarAsync("Cliente Teste");

        await Page.WaitForURLAsync("**/Conta/Editar**");
        await contaForm.PreencherAsync("Cliente Alterado", "Mesa01", "João");
        await contaForm.ConfirmarAsync();

        await Expect(listarPage.NomeDoConta("Cliente Alterado")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task NaoDeveEditar_Conta_Fechada()
    {
        await RegistrarEEntrarAsync("conta.editar.fechada@teste.local", "Senha123!");

        MesaFormPage mesaForm = new(Page, UrlBase);
        await mesaForm.IrParaCadastroAsync();
        await mesaForm.PreencherAsync("Mesa01", "4");
        await mesaForm.ConfirmarAsync();

        GarcomFormPage garcomForm = new(Page, UrlBase);
        await garcomForm.IrParaCadastroAsync();
        await garcomForm.PreencherAsync("João", "(00) 00000-0000", "111.111.111-11");
        await garcomForm.ConfirmarAsync();

        ContaFormPage contaForm = new(Page, UrlBase);
        await contaForm.IrParaCadastroAsync();
        await contaForm.PreencherAsync("Cliente Teste", "Mesa01", "João");
        await contaForm.ConfirmarAsync();

        ContaListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();
        await listarPage.ExcluirAsync("Cliente Teste");

        ContaExcluirPage excluirPage = new(Page);
        await excluirPage.ConfirmarAsync();

        await Expect(listarPage.NomeDoConta("Cliente Teste")).Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task DeveFechar_Conta_LiberandoMesa()
    {
        await RegistrarEEntrarAsync("conta.fechar@teste.local", "Senha123!");

        MesaFormPage mesaForm = new(Page, UrlBase);
        await mesaForm.IrParaCadastroAsync();
        await mesaForm.PreencherAsync("Mesa01", "4");
        await mesaForm.ConfirmarAsync();

        GarcomFormPage garcomForm = new(Page, UrlBase);
        await garcomForm.IrParaCadastroAsync();
        await garcomForm.PreencherAsync("João", "(00) 00000-0000", "111.111.111-11");
        await garcomForm.ConfirmarAsync();

        ContaFormPage contaForm = new(Page, UrlBase);
        await contaForm.IrParaCadastroAsync();
        await contaForm.PreencherAsync("Cliente Teste", "Mesa01", "João");
        await contaForm.ConfirmarAsync();

        ContaListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();
        await listarPage.ExcluirAsync("Cliente Teste");

        ContaExcluirPage excluirPage = new(Page);
        await excluirPage.ConfirmarAsync();

        MesaListarPage mesaListar = new(Page, UrlBase);
        await mesaListar.IrParaAsync();
        await Expect(mesaListar.StatusDaMesa("Mesa01")).ToHaveTextAsync("Livre");
    }

    [TestMethod]
    public async Task DeveListar_TodasAsContas()
    {
        await RegistrarEEntrarAsync("conta.listar@teste.local", "Senha123!");

        MesaFormPage mesaForm = new(Page, UrlBase);
        await mesaForm.IrParaCadastroAsync();
        await mesaForm.PreencherAsync("Mesa01", "4");
        await mesaForm.ConfirmarAsync();
        await mesaForm.IrParaCadastroAsync();
        await mesaForm.PreencherAsync("Mesa02", "6");
        await mesaForm.ConfirmarAsync();

        GarcomFormPage garcomForm = new(Page, UrlBase);
        await garcomForm.IrParaCadastroAsync();
        await garcomForm.PreencherAsync("João", "(00) 00000-0000", "111.111.111-11");
        await garcomForm.ConfirmarAsync();

        ContaFormPage contaForm = new(Page, UrlBase);
        await contaForm.IrParaCadastroAsync();
        await contaForm.PreencherAsync("Cliente 1", "Mesa01", "João");
        await contaForm.ConfirmarAsync();
        await contaForm.IrParaCadastroAsync();
        await contaForm.PreencherAsync("Cliente 2", "Mesa02", "João");
        await contaForm.ConfirmarAsync();

        ContaListarPage listarPage = new(Page, UrlBase);
        await listarPage.IrParaAsync();

        await Expect(listarPage.NomeDoConta("Cliente 1")).ToBeVisibleAsync();
        await Expect(listarPage.NomeDoConta("Cliente 2")).ToBeVisibleAsync();
    }
}
