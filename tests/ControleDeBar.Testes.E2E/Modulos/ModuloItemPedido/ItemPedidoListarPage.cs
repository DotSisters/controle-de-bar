using ControleDeBar.Testes.E2E.Modulos.ModuloConta;
using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloItemPedido;

public sealed class ItemPedidoListarPage(
    IPage page,
    string urlBase
)
{
    private readonly ContaListarPage contaListarPage = new(page, urlBase);

    public ILocator EstadoVazio => page.GetByText(
        "Nenhum pedido cadastrado nesta conta.",
        new() { Exact = false }
    );

    public ILocator ValorTotalDaConta => DdPorRotulo("Valor Total");

    public ILocator SituacaoDaConta => DdPorRotulo("Situação");

    public ILocator LinhaPorProduto(string nome) =>
        page.Locator("table tbody tr").Filter(
            new()
            {
                HasText = nome
            }
        );

    public ILocator NomeDoProduto(string nome) =>
        LinhaPorProduto(nome).Locator("td").Nth(0);

    public ILocator Quantidade(string nome) =>
        LinhaPorProduto(nome).Locator("td").Nth(1);

    public ILocator ValorUnitario(string nome) =>
        LinhaPorProduto(nome).Locator("td").Nth(2);

    public ILocator TotalDoItem(string nome) =>
        LinhaPorProduto(nome).Locator("td").Nth(3);

    public ILocator BotaoAlterar(string nome) =>
        LinhaPorProduto(nome).GetByRole(
            AriaRole.Button,
            new()
            {
                Name = "Alterar",
                Exact = true
            }
        );

    public ILocator BotaoRemover(string nome) =>
        LinhaPorProduto(nome).GetByRole(
            AriaRole.Button,
            new()
            {
                Name = "Remover",
                Exact = true
            }
        );

    public async Task IrParaAsync(string nomeCliente)
    {
        await contaListarPage.AdicionarPedidosAsync(nomeCliente);
    }

    public async Task IrParaUrlAsync(string url)
    {
        await page.GotoAsync(url);
    }

    public async Task ExcluirAsync(string nomeProduto)
    {
        await BotaoRemover(nomeProduto).ClickAsync();
    }

    private ILocator DdPorRotulo(string rotulo) =>
        page.Locator("dt").Filter(
            new()
            {
                HasText = rotulo
            }
        ).Locator("xpath=following-sibling::dd[1]");
}
