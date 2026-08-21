using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloConta;

public sealed class ContaListarPage(
    IPage page,
    string urlBase
)
{
    public string Url => $"{urlBase}/Conta/Listar";

    public ILocator Titulo => page.GetByRole(
        AriaRole.Heading,
        new()
        {
            Name = "Contas",
            Exact = false
        }
    );

    public ILocator CadastrarNovo => page.GetByRole(
        AriaRole.Link,
        new()
        {
            Name = "Cadastrar Conta",
            Exact = false
        }
    );

    public ILocator EstadoVazio => page.GetByText(
        "Nenhuma conta cadastrada.",
        new()
        {
            Exact = false
        }
    );

    public ILocator MensagemErro => page.Locator(".alert-danger");

    public ILocator NomeDoConta(string nome) => page.GetByRole(
        AriaRole.Heading,
        new()
        {
            Name = nome,
            Exact = true
        }
    );

    public ILocator NomeDoCliente(string nome) => NomeDoConta(nome);

    public ILocator StatusDaConta(string cliente)
    {
        ILocator card = CardPorNome(cliente);
        return card.GetByText("Aberta", new() { Exact = false })
                   .Or(card.GetByText("Fechada", new() { Exact = false }));
    }

    public ILocator StatusDaMesa(string identificacaoMesa)
    {
        return page.GetByText($"Mesa {identificacaoMesa}")
                   .Locator("..")
                   .Locator(".status");
    }

    public ILocator Total(string cliente)
    {
        return CardPorNome(cliente).Locator(".total");
    }

    public ILocator Pedido(string cliente, string produto)
    {
        return CardPorNome(cliente).GetByText(produto);
    }

    public ILocator LinkAdicionarPedidos(string nomeCliente) =>
        CardPorNome(nomeCliente).GetByRole(
            AriaRole.Link,
            new()
            {
                Name = "Adicionar Pedidos",
                Exact = false
            }
        );

    public ILocator TotalDaConta(string nomeCliente)
    {
        return CardPorNome(nomeCliente)
            .Locator("dd")
            .Nth(3);
    }

    public async Task IrParaAsync()
    {
        await page.GotoAsync(Url);
    }

    public async Task EditarAsync(string nome)
    {
        ILocator linkEditar = CardPorNome(nome).GetByRole(
            AriaRole.Link,
            new()
            {
                Name = "Editar",
                Exact = false
            }
        );

        await linkEditar.ClickAsync();
    }

    public async Task ExcluirAsync(string nome)
    {
        ILocator linkExcluir = CardPorNome(nome).GetByRole(
            AriaRole.Link,
            new()
            {
                Name = "Excluir",
                Exact = false
            }
        );

        await linkExcluir.ClickAsync();
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
        ILocator card = CardPorNome(nomeCliente);

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

    private ILocator CardPorNome(string nome)
    {
        ILocator nomeConta = NomeDoConta(nome);
        return page.Locator(".card").Filter(
            new()
            {
                Has = nomeConta
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
