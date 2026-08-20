using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloGarcom;

public sealed class GarcomFormPage(
    IPage page,
    string urlBase
)
{
    public string UrlCadastrar => $"{urlBase}/Garcom/Cadastrar";

    public string UrlEditar => $"{urlBase}/Garcom/Editar";

    public ILocator Nome =>
        page.GetByLabel("Nome");

    public ILocator Telefone =>
        page.GetByLabel("Telefone");

    public ILocator Cpf =>
        page.GetByLabel("CPF");

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
        string telefone,
        string cpf
    )
    {
        await Nome.FillAsync(nome);
        await Telefone.FillAsync(telefone);
        await Cpf.FillAsync(cpf);
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
