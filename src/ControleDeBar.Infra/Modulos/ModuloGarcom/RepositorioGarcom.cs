using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Infra.Compartilhado.Orm;

public sealed class RepositorioGarcom(
    ControleDeBarDbContext dbContext
) : RepositorioBase<Garcom>(dbContext), IRepositorioGarcom;
