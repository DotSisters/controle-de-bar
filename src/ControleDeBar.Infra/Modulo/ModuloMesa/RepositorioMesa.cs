using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Infra.Compartilhado.Orm;

public sealed class RepositorioMesa(
    ControleDeBarDbContext dbContext
) : RepositorioBase<Mesa>(dbContext), IRepositorioMesa;