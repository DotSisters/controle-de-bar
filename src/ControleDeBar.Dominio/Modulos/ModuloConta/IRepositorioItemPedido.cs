using ControleDeBar.Dominio.Compartilhado;

namespace ControleDeBar.Dominio.Modulos.ModuloConta;

public interface IRepositorioItemPedido : IRepositorio<ItemPedido>
{
    bool ExistePorProduto(Guid produtoId);
    List<ItemPedido> SelecionarPorConta(Guid contaId);
}
