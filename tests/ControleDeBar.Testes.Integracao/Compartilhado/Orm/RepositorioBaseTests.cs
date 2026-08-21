using ControleDeBar.Infra.Compartilhado.Orm;
using ControleDeBar.Infra.Modulos.ModuloConta;
using ControleDeBar.Testes.Integracao.Compartilhado.Identity;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Testes.Integracao.Compartilhado.Orm;

public abstract class RepositorioBaseTests
{
    protected ControleDeBarDbContext dbContext = null!;
    protected RepositorioMesa repositorioMesa = null!;
    protected RepositorioGarcom repositorioGarcom = null!;
    protected RepositorioProduto repositorioProduto = null!;
    protected RepositorioConta repositorioConta = null!;
    protected RepositorioItemPedido repositorioItemPedido = null!;

    [TestInitialize]
    public void InicializarContexto()
    {
        dbContext = CriarDbContext(Guid.NewGuid());

        repositorioMesa = new RepositorioMesa(dbContext);
        repositorioGarcom = new RepositorioGarcom(dbContext);
        repositorioProduto = new RepositorioProduto(dbContext);
        repositorioConta = new RepositorioConta(dbContext);
        repositorioItemPedido = new RepositorioItemPedido(dbContext);
    }

    [TestCleanup]
    public void DescartarContexto()
    {
        dbContext.Dispose();
    }

    private static ControleDeBarDbContext CriarDbContext(Guid userId)
    {
        DbContextOptions<ControleDeBarDbContext> options =
            new DbContextOptionsBuilder<ControleDeBarDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

        return new ControleDeBarDbContext(
            options,
            new ProvedorDeUsuarioFake(userId)
        );
    }
}
