using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloMesa;

public sealed class MesaListarPage(
    IPage page,
    string urlBase
)
{
    public string Url => $"{urlBase}/Mesa/Listar";

    public ILocator Titulo => page.GetByRole(
        AriaRole.Heading,
        new()
        {
            Name = "Mesas",
            Exact = false
        }
    );

    public ILocator CadastrarNovo => page.GetByRole(
        AriaRole.Link,
        new()
        {
            Name = "Cadastrar Mesa",
            Exact = false
        }
    );

    public ILocator EstadoVazio => page.GetByText(
        "Nenhuma mesa cadastrada.",
        new()
        {
            Exact = false
        }
    );

    public ILocator MensagemErro => page.Locator(".alert-danger");

    // O título renderizado é "Mesa Mesa01"
    public ILocator NomeDaMesa(string nome) => page.GetByRole(
        AriaRole.Heading,
        new()
        {
            Name = $"Mesa {nome}",
            Exact = true
        }
    );

    public ILocator QuantidadeDeLugares(string nome)
    {
        return CardPorNome(nome)
            .Locator("dd")
            .Nth(0);
    }

    public ILocator StatusDaMesa(string nome)
    {
        return CardPorNome(nome)
            .Locator("dd")
            .Nth(1);
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
        ILocator nomeMesa = NomeDaMesa(nome);
        return page.Locator(".card").Filter(
            new()
            {
                Has = nomeMesa
            }
        );
    }
}
