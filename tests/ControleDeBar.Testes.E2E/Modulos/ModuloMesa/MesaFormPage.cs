using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloMesa;

public sealed class MesaFormPage(
    IPage page,
    string urlBase
)
{
    public string UrlCadastrar => $"{urlBase}/Mesa/Cadastrar";

    public string UrlEditar => $"{urlBase}/Mesa/Editar";

    public ILocator Identificacao =>
        page.GetByLabel("Identificação");

    public ILocator QuantidadeLugar =>
        page.GetByLabel("Quantidade de Lugares");

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
        string identificacao,
        string quantidadeLugar
    )
    {
        await Identificacao.FillAsync(identificacao);
        await QuantidadeLugar.FillAsync(quantidadeLugar);
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