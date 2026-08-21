using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloConta;

public sealed class ContaGerenciarPage(
    IPage page,
    string urlBase
)
{
    public string Url => $"{urlBase}/Conta/Gerenciar";

    public ILocator ValorTotal => DdPorRotulo("Valor Total");

    public ILocator Situacao => DdPorRotulo("Situação");

    public ILocator BotaoFecharConta => page.GetByRole(
        AriaRole.Button,
        new()
        {
            Name = "Fechar Conta",
            Exact = true
        }
    );

    public ILocator LinhaPorProduto(string nome) =>
        page.Locator("table tbody tr").Filter(
            new()
            {
                HasText = nome
            }
        );

    public ILocator Quantidade(string nome) =>
        LinhaPorProduto(nome).Locator("td").Nth(1);

    public ILocator ValorUnitario(string nome) =>
        LinhaPorProduto(nome).Locator("td").Nth(2);

    public ILocator TotalDoItem(string nome) =>
        LinhaPorProduto(nome).Locator("td").Nth(3);

    public async Task IrParaAsync(Guid id)
    {
        await page.GotoAsync($"{Url}/{id}");
    }

    public async Task FecharAsync()
    {
        await BotaoFecharConta.ClickAsync();
    }

    private ILocator DdPorRotulo(string rotulo) =>
        page.Locator("dt").Filter(
            new()
            {
                HasText = rotulo
            }
        ).Locator("xpath=following-sibling::dd[1]");
}
