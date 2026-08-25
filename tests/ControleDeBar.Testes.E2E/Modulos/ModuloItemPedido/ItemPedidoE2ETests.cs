using ControleDeBar.Testes.E2E.Compartilhado;
using ControleDeBar.Testes.E2E.Modulos.ModuloConta;
using ControleDeBar.Testes.E2E.Modulos.ModuloGarcom;
using ControleDeBar.Testes.E2E.Modulos.ModuloMesa;
using ControleDeBar.Testes.E2E.Modulos.ModuloProduto;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloItemPedido;

[TestClass]
public sealed class ItemPedidoE2ETests : E2ETestsBase
{
    private const string NomeCliente = "Cliente";
    private const string IdentificacaoMesa = "Mesa01";
    private const string NomeGarcom = "Garcom01";

    [TestMethod]
    public async Task DeveAdicionar_ItemPedido_AContaAberta_ComDadosValidos()
    {
        await RegistrarEEntrarAsync(
            "itempedido.cadastro@teste.local",
            "Senha123!"
        );

        await CadastrarDependenciasDaContaAsync(("Produto01", 10.00m));

        ItemPedidoFormPage formPage = new(Page, UrlBase);
        ItemPedidoListarPage listarPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync(NomeCliente);
        await formPage.PreencherAsync("Produto01", "2");
        await formPage.ConfirmarAsync();

        await Expect(listarPage.LinhaPorProduto("Produto01"))
            .ToBeVisibleAsync();

        await Expect(listarPage.Quantidade("Produto01"))
            .ToHaveTextAsync("2");

        await Expect(listarPage.ValorUnitario("Produto01"))
            .ToContainTextAsync("R$ 10.00");

        await Expect(listarPage.TotalDoItem("Produto01"))
            .ToContainTextAsync("R$ 20.00");

        await Expect(listarPage.ValorTotalDaConta)
            .ToContainTextAsync("R$ 20.00");
    }

    [TestMethod]
    public async Task DeveImpedirAdicionar_ItemPedido_AContaFechada()
    {
        await RegistrarEEntrarAsync(
            "itempedido.adicionar.fechada@teste.local",
            "Senha123!"
        );

        await CadastrarDependenciasDaContaAsync(("Produto01", 10.00m));

        ItemPedidoFormPage formPage = new(Page, UrlBase);
        ItemPedidoListarPage listarPage = new(Page, UrlBase);
        ContaListarPage contaListarPage = new(Page, UrlBase);
        ContaGerenciarPage gerenciarPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync(NomeCliente);
        await formPage.PreencherAsync("Produto01", "1");
        await formPage.ConfirmarAsync();

        await contaListarPage.IrParaAsync();
        string urlAdicionarPedidos = await contaListarPage.ObterUrlAdicionarPedidosAsync(NomeCliente);

        await contaListarPage.IrParaGerenciarAsync(NomeCliente);
        await gerenciarPage.FecharAsync();

        await formPage.IrParaUrlAsync(urlAdicionarPedidos);

        await Expect(formPage.MensagemContaFechada)
            .ToBeVisibleAsync();

        await Expect(formPage.Produto)
            .Not.ToBeVisibleAsync();

        await Expect(formPage.BotaoAdicionarPedido)
            .Not.ToBeVisibleAsync();

        await Expect(listarPage.LinhaPorProduto("Produto01"))
            .ToBeVisibleAsync();

        await Expect(listarPage.Quantidade("Produto01"))
            .ToHaveTextAsync("1");

        await Expect(listarPage.TotalDoItem("Produto01"))
            .ToContainTextAsync("R$ 10.00");

        await Expect(listarPage.ValorTotalDaConta)
            .ToContainTextAsync("R$ 10.00");

        await Expect(listarPage.SituacaoDaConta)
            .ToHaveTextAsync("Fechada");
    }

    [TestMethod]
    public async Task DeveListar_TodosOsItemPedido_DeContaAberta()
    {
        await RegistrarEEntrarAsync(
            "itempedido.listar@teste.local",
            "Senha123!"
        );

        await CadastrarDependenciasDaContaAsync(
            ("Produto01", 10.00m),
            ("Produto02", 15.00m)
        );

        ItemPedidoFormPage formPage = new(Page, UrlBase);
        ItemPedidoListarPage listarPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync(NomeCliente);
        await formPage.PreencherAsync("Produto01", "2");
        await formPage.ConfirmarAsync();

        await formPage.PreencherAsync("Produto02", "1");
        await formPage.ConfirmarAsync();

        await Expect(listarPage.LinhaPorProduto("Produto01"))
            .ToBeVisibleAsync();

        await Expect(listarPage.Quantidade("Produto01"))
            .ToHaveTextAsync("2");

        await Expect(listarPage.ValorUnitario("Produto01"))
            .ToContainTextAsync("R$ 10.00");

        await Expect(listarPage.TotalDoItem("Produto01"))
            .ToContainTextAsync("R$ 20.00");

        await Expect(listarPage.LinhaPorProduto("Produto02"))
            .ToBeVisibleAsync();

        await Expect(listarPage.Quantidade("Produto02"))
            .ToHaveTextAsync("1");

        await Expect(listarPage.ValorUnitario("Produto02"))
            .ToContainTextAsync("R$ 15.00");

        await Expect(listarPage.TotalDoItem("Produto02"))
            .ToContainTextAsync("R$ 15.00");

        await Expect(listarPage.ValorTotalDaConta)
            .ToContainTextAsync("R$ 35.00");
    }

    [TestMethod]
    public async Task DeveAlterarQuantidade_ItemPedido_EmContaAberta()
    {
        await RegistrarEEntrarAsync(
            "itempedido.alterar@teste.local",
            "Senha123!"
        );

        await CadastrarDependenciasDaContaAsync(("Produto01", 10.00m));

        ItemPedidoFormPage formPage = new(Page, UrlBase);
        ItemPedidoListarPage listarPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync(NomeCliente);
        await formPage.PreencherAsync("Produto01", "2");
        await formPage.ConfirmarAsync();

        await formPage.AlterarQuantidadeAsync("Produto01", "5");

        await Expect(listarPage.Quantidade("Produto01"))
            .ToHaveTextAsync("5");

        await Expect(listarPage.TotalDoItem("Produto01"))
            .ToContainTextAsync("R$ 50.00");

        await Expect(listarPage.ValorTotalDaConta)
            .ToContainTextAsync("R$ 50.00");
    }

    [TestMethod]
    public async Task DeveImpedirAlterar_ItemPedido_DeContaFechada()
    {
        await RegistrarEEntrarAsync(
            "itempedido.alterar.fechada@teste.local",
            "Senha123!"
        );

        await CadastrarDependenciasDaContaAsync(("Produto01", 10.00m));

        ItemPedidoFormPage formPage = new(Page, UrlBase);
        ItemPedidoListarPage listarPage = new(Page, UrlBase);
        ContaListarPage contaListarPage = new(Page, UrlBase);
        ContaGerenciarPage gerenciarPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync(NomeCliente);
        await formPage.PreencherAsync("Produto01", "2");
        await formPage.ConfirmarAsync();

        await contaListarPage.IrParaAsync();
        string urlAdicionarPedidos = await contaListarPage.ObterUrlAdicionarPedidosAsync(NomeCliente);

        await contaListarPage.IrParaGerenciarAsync(NomeCliente);
        await gerenciarPage.FecharAsync();

        await formPage.IrParaUrlAsync(urlAdicionarPedidos);

        await Expect(listarPage.BotaoAlterar("Produto01"))
            .Not.ToBeVisibleAsync();

        await Expect(listarPage.Quantidade("Produto01"))
            .ToHaveTextAsync("2");

        await Expect(listarPage.TotalDoItem("Produto01"))
            .ToContainTextAsync("R$ 20.00");

        await Expect(listarPage.ValorTotalDaConta)
            .ToContainTextAsync("R$ 20.00");

        await contaListarPage.IrParaGerenciarAsync(NomeCliente);

        await Expect(gerenciarPage.Quantidade("Produto01"))
            .ToHaveTextAsync("2");

        await Expect(gerenciarPage.TotalDoItem("Produto01"))
            .ToContainTextAsync("R$ 20.00");

        await Expect(gerenciarPage.ValorTotal)
            .ToContainTextAsync("R$ 20.00");
    }

    [TestMethod]
    public async Task DeveRemover_ItemPedido_DeContaAberta()
    {
        await RegistrarEEntrarAsync(
            "itempedido.remover@teste.local",
            "Senha123!"
        );

        await CadastrarDependenciasDaContaAsync(
            ("Produto01", 10.00m),
            ("Produto02", 15.00m)
        );

        ItemPedidoFormPage formPage = new(Page, UrlBase);
        ItemPedidoListarPage listarPage = new(Page, UrlBase);
        ItemPedidoExcluirPage excluirPage = new(Page);

        await formPage.IrParaCadastroAsync(NomeCliente);
        await formPage.PreencherAsync("Produto01", "2");
        await formPage.ConfirmarAsync();

        await formPage.PreencherAsync("Produto02", "1");
        await formPage.ConfirmarAsync();

        await excluirPage.ConfirmarAsync("Produto01");

        await Expect(listarPage.LinhaPorProduto("Produto01"))
            .Not.ToBeVisibleAsync();

        await Expect(listarPage.LinhaPorProduto("Produto02"))
            .ToBeVisibleAsync();

        await Expect(listarPage.TotalDoItem("Produto02"))
            .ToContainTextAsync("R$ 15.00");

        await Expect(listarPage.ValorTotalDaConta)
            .ToContainTextAsync("R$ 15.00");
    }

    [TestMethod]
    public async Task DeveImpedirRemover_ItemPedido_DeContaFechada()
    {
        await RegistrarEEntrarAsync(
            "itempedido.remover.fechada@teste.local",
            "Senha123!"
        );

        await CadastrarDependenciasDaContaAsync(("Produto01", 10.00m));

        ItemPedidoFormPage formPage = new(Page, UrlBase);
        ItemPedidoListarPage listarPage = new(Page, UrlBase);
        ContaListarPage contaListarPage = new(Page, UrlBase);
        ContaGerenciarPage gerenciarPage = new(Page, UrlBase);

        await formPage.IrParaCadastroAsync(NomeCliente);
        await formPage.PreencherAsync("Produto01", "1");
        await formPage.ConfirmarAsync();

        await contaListarPage.IrParaAsync();
        string urlAdicionarPedidos = await contaListarPage.ObterUrlAdicionarPedidosAsync(NomeCliente);

        await contaListarPage.IrParaGerenciarAsync(NomeCliente);
        await gerenciarPage.FecharAsync();

        await formPage.IrParaUrlAsync(urlAdicionarPedidos);

        await Expect(listarPage.BotaoRemover("Produto01"))
            .Not.ToBeVisibleAsync();

        await Expect(listarPage.LinhaPorProduto("Produto01"))
            .ToBeVisibleAsync();

        await Expect(listarPage.Quantidade("Produto01"))
            .ToHaveTextAsync("1");

        await Expect(listarPage.TotalDoItem("Produto01"))
            .ToContainTextAsync("R$ 10.00");

        await Expect(listarPage.ValorTotalDaConta)
            .ToContainTextAsync("R$ 10.00");
    }

    private async Task CadastrarDependenciasDaContaAsync(
        params (string Nome, decimal Valor)[] produtos
    )
    {
        (string Nome, decimal Valor)[] produtosParaCadastrar = produtos.Length == 0
            ? [("Produto01", 10.00m)]
            : produtos;

        GarcomFormPage garcomFormPage = new(Page, UrlBase);

        await garcomFormPage.IrParaCadastroAsync();
        await garcomFormPage.PreencherAsync(NomeGarcom, "(00) 00000-0000", "111.111.111-11");
        await garcomFormPage.ConfirmarAsync();

        MesaFormPage mesaFormPage = new(Page, UrlBase);

        await mesaFormPage.IrParaCadastroAsync();
        await mesaFormPage.PreencherAsync(IdentificacaoMesa, "4");
        await mesaFormPage.ConfirmarAsync();

        ProdutoFormPage produtoFormPage = new(Page, UrlBase);

        foreach ((string nome, decimal valor) in produtosParaCadastrar)
        {
            await produtoFormPage.IrParaCadastroAsync();
            await produtoFormPage.PreencherAsync(nome, valor);
            await produtoFormPage.ConfirmarAsync();
        }

        ContaFormPage contaFormPage = new(Page, UrlBase);

        await contaFormPage.IrParaCadastroAsync();
        await contaFormPage.PreencherAsync(NomeCliente, IdentificacaoMesa, NomeGarcom);
        await contaFormPage.ConfirmarAsync();
    }
}
