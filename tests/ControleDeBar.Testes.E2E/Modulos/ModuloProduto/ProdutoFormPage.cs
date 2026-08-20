using System.Globalization;
using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloProduto;

public sealed class ProdutoFormPage(
    IPage page,
    string urlBase
)
{
    public string UrlCadastrar => $"{urlBase}/Produto/Cadastrar";

    public string UrlEditar => $"{urlBase}/Produto/Editar";

    public ILocator Nome =>
        page.GetByLabel("Nome");

    public ILocator Valor =>
        page.GetByLabel("Valor");

    public ILocator MensagemErro(string mensagem) =>
        page.GetByText(mensagem, new() { Exact = true });

    public async Task IrParaCadastroAsync()
    {
        await page.GotoAsync(UrlCadastrar);
    }

    public async Task IrParaEdicaoAsync(Guid id)
    {
        await page.GotoAsync($"{UrlEditar}?id={id}");
    }

    public async Task PreencherAsync(
        string nome,
        decimal valor
    )
    {
        await Nome.FillAsync(nome);
        await Valor.FillAsync(valor.ToString("F2", CultureInfo.InvariantCulture));
    }

    public async Task ConfirmarAsync()
    {
        await page.GetByRole(
            AriaRole.Button,
            new()
            {
                Name = "Confirmar",
                Exact = true
            }
        ).ClickAsync();
    }
}