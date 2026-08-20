using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloConta;

public sealed class ContaFormPage(
    IPage page,
    string urlBase
)
{
    public string UrlCadastrar => $"{urlBase}/Conta/Cadastrar";

    public ILocator NomeCliente =>
        page.GetByLabel("Nome do Cliente");

    public ILocator Mesa =>
        page.GetByLabel("Mesa");

    public ILocator Garcom =>
        page.GetByLabel("Garçom");

    public async Task IrParaCadastroAsync()
    {
        await page.GotoAsync(UrlCadastrar);
    }

    public async Task PreencherAsync(
        string nomeCliente,
        string identificacaoMesa,
        string nomeGarcom
    )
    {
        await NomeCliente.FillAsync(nomeCliente);

        await Mesa.SelectOptionAsync(new SelectOptionValue
        {
            Label = identificacaoMesa
        });

        await Garcom.SelectOptionAsync(new SelectOptionValue
        {
            Label = nomeGarcom
        });
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
