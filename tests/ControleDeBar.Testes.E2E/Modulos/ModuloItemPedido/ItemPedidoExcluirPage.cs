using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloItemPedido;

public sealed class ItemPedidoExcluirPage(
    IPage page
)
{
    public async Task ConfirmarAsync(string nomeProduto)
    {
        ILocator linha = page.Locator("table tbody tr").Filter(
            new()
            {
                HasText = nomeProduto
            }
        );

        await linha.GetByRole(
            AriaRole.Button,
            new()
            {
                Name = "Remover",
                Exact = true
            }
        ).ClickAsync();
    }
}
