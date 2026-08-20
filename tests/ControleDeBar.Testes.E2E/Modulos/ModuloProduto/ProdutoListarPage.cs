using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloProduto;

public sealed class ProdutoListarPage(
    IPage page,
    string urlBase
)
{
    public string Url => $"{urlBase}/Produto/Listar";

    public ILocator Titulo => page.GetByRole(
        AriaRole.Heading,
        new() { Name = "Produtos", Exact = false }
    );

    public ILocator CadastrarNovo => page.GetByRole(
        AriaRole.Link,
        new() { Name = "Cadastrar Produto", Exact = false }
    );

    public ILocator EstadoVazio => page.GetByText(
        "Nenhum produto cadastrado.",
        new() { Exact = false }
    );

    public ILocator MensagemErro => page.Locator(".alert-danger");

    public ILocator NomeDoProduto(string nome) => page.GetByRole(
        AriaRole.Heading,
        new() { Name = nome, Exact = true }
    );

    public ILocator Valor(string nome)
    {
        return CardPorNome(nome)
            .Locator("dd")
            .Filter(new() { HasText = "R$" });
    }

    public async Task IrParaAsync() => await page.GotoAsync(Url);

    public async Task EditarAsync(string nome)
    {
        ILocator linkEditar = CardPorNome(nome).GetByRole(
            AriaRole.Link,
            new() { Name = "Editar", Exact = false }
        );

        await linkEditar.ClickAsync();
    }

    public async Task ExcluirAsync(string nome)
    {
        ILocator linkExcluir = CardPorNome(nome).GetByRole(
            AriaRole.Link,
            new() { Name = "Excluir", Exact = false }
        );

        await linkExcluir.ClickAsync();
    }

    private ILocator CardPorNome(string nome)
    {
        ILocator nomeProduto = NomeDoProduto(nome);
        return page.Locator(".card").Filter(new() { Has = nomeProduto });
    }
}
