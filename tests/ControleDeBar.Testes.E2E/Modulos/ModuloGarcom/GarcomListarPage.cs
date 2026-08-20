using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloGarcom;

public sealed class GarcomListarPage(
    IPage page,
    string urlBase
)
{
    public string Url => $"{urlBase}/Garcom/Listar";

    public ILocator Titulo => page.GetByRole(
        AriaRole.Heading,
        new()
        {
            Name = "Garçons",
            Exact = false
        }
    );

    public ILocator CadastrarNovo => page.GetByRole(
        AriaRole.Link,
        new()
        {
            Name = "Cadastrar Garçom",
            Exact = false
        }
    );

    public ILocator EstadoVazio => page.GetByText(
        "Nenhum garçom cadastrado.",
        new()
        {
            Exact = false
        }
    );

    public ILocator MensagemErro => page.Locator(".alert-danger");

    public ILocator NomeDoGarcom(string nome) => page.GetByRole(
        AriaRole.Heading,
        new()
        {
            Name = nome,
            Exact = true
        }
    );

    public ILocator TelefoneDoGarcom(string nome)
    {
        return CardPorNome(nome)
            .Locator("dd")
            .Nth(0);
    }

    public ILocator CpfDoGarcom(string nome)
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
        ILocator nomeGarcom = NomeDoGarcom(nome);
        return page.Locator(".card").Filter(
            new()
            {
                Has = nomeGarcom
            }
        );
    }
}
