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

    // Novo método para capturar o status da conta (Aberta/Fechada)
    public ILocator StatusDaConta(string cliente)
    {
        ILocator card = CardPorNome(cliente);
        return card.GetByText("Aberta", new() { Exact = false })
                   .Or(card.GetByText("Fechada", new() { Exact = false }));
    }

    public ILocator StatusDaMesa(string identificacaoMesa)
    {
        // Ajuste conforme o HTML real da sua aplicação
        return page.GetByText($"Mesa {identificacaoMesa}")
                   .Locator("..")
                   .Locator(".status");
    }

    public ILocator Total(string cliente)
    {
        // Localiza o total dentro do card da conta
        return CardPorNome(cliente).Locator(".total");
    }

    public ILocator Pedido(string cliente, string produto)
    {
        // Localiza o pedido pelo nome do produto dentro do card da conta
        return CardPorNome(cliente).GetByText(produto);
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
}
