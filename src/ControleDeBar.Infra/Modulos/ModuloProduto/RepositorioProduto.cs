using ControleDeBar.Dominio.Modulos.ModuloProduto;
using ControleDeBar.Infra.Compartilhado.Orm;

public sealed class RepositorioProduto(
    ControleDeBarDbContext dbContext
) : RepositorioBase<Produto>(dbContext), IRepositorioProduto;