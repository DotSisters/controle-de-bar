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

    // Novo: captura mensagens de erro exibidas na tela de cadastro
    public ILocator MensagemErro =>
        page.GetByText("Não é possível abrir uma conta para esta mesa porque ela já possui uma conta em aberto.", new() { Exact = false });


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
