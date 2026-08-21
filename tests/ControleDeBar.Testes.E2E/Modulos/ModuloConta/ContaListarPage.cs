using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloConta;

public sealed class ContaListarPage(
    IPage page,
    string urlBase
)
{
    public string Url => $"{urlBase}/Conta/Listar";

    public ILocator NomeDoCliente(string nome) => page.GetByRole(
        AriaRole.Heading,
        new()
        {
            Name = nome,
            Exact = true
        }
    );

    public ILocator LinkAdicionarPedidos(string nomeCliente) =>
        CardPorCliente(nomeCliente).GetByRole(
            AriaRole.Link,
            new()
            {
                Name = "Adicionar Pedidos",
                Exact = false
            }
        );

    public ILocator TotalDaConta(string nomeCliente)
    {
        return CardPorCliente(nomeCliente)
            .Locator("dd")
            .Nth(3);
    }

    public async Task IrParaAsync()
    {
        await page.GotoAsync(Url);
    }

    public async Task AdicionarPedidosAsync(string nomeCliente)
    {
        await IrParaAsync();
        await LinkAdicionarPedidos(nomeCliente).ClickAsync();
    }

    public async Task IrParaGerenciarAsync(string nomeCliente)
    {
        await IrParaAsync();

        await LinkGerenciar(nomeCliente).ClickAsync();
    }

    public async Task<string> ObterUrlAdicionarPedidosAsync(string nomeCliente)
    {
        ILocator linkAdicionar = LinkAdicionarPedidos(nomeCliente);

        if (await linkAdicionar.CountAsync() > 0)
            return ResolverUrl(await linkAdicionar.GetAttributeAsync("href"));

        string urlGerenciar = ResolverUrl(
            await LinkGerenciar(nomeCliente).GetAttributeAsync("href")
        );

        return urlGerenciar.Replace(
            "/Conta/Gerenciar/",
            "/Conta/AdicionarPedidos/",
            StringComparison.OrdinalIgnoreCase
        );
    }

    private ILocator LinkGerenciar(string nomeCliente)
    {
        ILocator card = CardPorCliente(nomeCliente);

        return card.GetByRole(
            AriaRole.Link,
            new()
            {
                Name = "Fechar Conta",
                Exact = false
            }
        ).Or(
            card.GetByRole(
                AriaRole.Link,
                new()
                {
                    Name = "Ver Detalhes",
                    Exact = false
                }
            )
        );
    }

    private ILocator CardPorCliente(string nomeCliente)
    {
        ILocator nome = NomeDoCliente(nomeCliente);

        return page.Locator(".card").Filter(
            new()
            {
                Has = nome
            }
        );
    }

    private string ResolverUrl(string? href)
    {
        if (string.IsNullOrWhiteSpace(href))
            return string.Empty;

        if (href.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            return href;

        if (href.StartsWith('/'))
            return $"{urlBase.TrimEnd('/')}{href}";

        return $"{urlBase.TrimEnd('/')}/{href}";
    }
}
