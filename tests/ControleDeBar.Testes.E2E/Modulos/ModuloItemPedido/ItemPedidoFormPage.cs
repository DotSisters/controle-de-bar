using ControleDeBar.Testes.E2E.Modulos.ModuloConta;
using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloItemPedido;

public sealed class ItemPedidoFormPage(
    IPage page,
    string urlBase
)
{
    private readonly ContaListarPage contaListarPage = new(page, urlBase);

    public ILocator Produto =>
        page.GetByLabel("Produto");

    public ILocator Quantidade =>
        page.GetByLabel("Quantidade");

    public ILocator BotaoAdicionarPedido => page.GetByRole(
        AriaRole.Button,
        new()
        {
            Name = "Adicionar Pedido",
            Exact = true
        }
    );

    public ILocator MensagemContaFechada => page.GetByText(
        "Não é possível adicionar pedidos a uma conta fechada.",
        new() { Exact = true }
    );

    public ILocator MensagemErro => page.Locator(".alert-danger");

    public async Task IrParaCadastroAsync(string nomeCliente)
    {
        await contaListarPage.AdicionarPedidosAsync(nomeCliente);
    }

    public async Task IrParaUrlAsync(string url)
    {
        await page.GotoAsync(url);
    }

    public async Task PreencherAsync(string nomeProduto, string quantidade)
    {
        ILocator option = Produto.Locator("option").Filter(
            new()
            {
                HasText = nomeProduto
            }
        );

        string? valor = await option.GetAttributeAsync("value");
        await Produto.SelectOptionAsync(valor ?? string.Empty);
        await Quantidade.FillAsync(quantidade);
    }

    public async Task ConfirmarAsync()
    {
        await BotaoAdicionarPedido.ClickAsync();
    }

    public async Task AlterarQuantidadeAsync(string nomeProduto, string quantidade)
    {
        ILocator linha = page.Locator("table tbody tr").Filter(
            new()
            {
                HasText = nomeProduto
            }
        );

        await linha.Locator("input[name='quantidade']").FillAsync(quantidade);

        await linha.GetByRole(
            AriaRole.Button,
            new()
            {
                Name = "Alterar",
                Exact = true
            }
        ).ClickAsync();
    }
}
