using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Infra.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Infra.Modulos.ModuloConta;

public sealed class RepositorioItemPedido(
    ControleDeBarDbContext dbContext
) : RepositorioBase<ItemPedido>(dbContext), IRepositorioItemPedido
{
    public override ItemPedido? SelecionarPorId(Guid idSelecionado)
    {
        return registros
            .Include(i => i.Conta)
            .Include(i => i.Produto)
            .SingleOrDefault(i => i.Id == idSelecionado);
    }

    public override List<ItemPedido> SelecionarTodos()
    {
        return registros
            .Include(i => i.Produto)
            .ToList();
    }

    public List<ItemPedido> SelecionarPorConta(Guid contaId)
    {
        return registros
            .Include(i => i.Produto)
            .Where(i => i.ContaId == contaId)
            .ToList();
    }

    public bool ExistePorProduto(Guid produtoId)
    {
        return registros.Any(i => i.ProdutoId == produtoId);
    }
}
