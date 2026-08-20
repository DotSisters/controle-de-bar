using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Infra.Compartilhado.Orm;
using ControleDeBar.Testes.Integracao.Compartilhado.Identity;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Testes.Integracao.Compartilhado.Orm;

public abstract class RepositorioBaseTests
{
    protected ControleDeBarDbContext dbContext = null!;
    protected RepositorioMesa repositorioMesa = null!;
    // protected RepositorioGarcom repositorioGarcom = null!;
    // protected RepositorioProduto repositorioProduto = null!;
    // protected RepositorioConta repositorioConta = null!;
    // protected RepositorioPedido repositorioPedido = null!;

    [TestInitialize]
    public void InicializarContexto()
    {
        dbContext = CriarDbContext(Guid.NewGuid());

        // Mesa
        repositorioMesa = new RepositorioMesa(dbContext);

        BuilderSetup.SetCreatePersistenceMethod<Mesa>(repositorioMesa.Cadastrar);
        BuilderSetup.SetCreatePersistenceMethod<IList<Mesa>>((mesas) =>
        {
            foreach (Mesa m in mesas)
                repositorioMesa.Cadastrar(m);
        });

        // // Garcom
        // repositorioGarcom = new RepositorioGarcom(dbContext);

        // BuilderSetup.SetCreatePersistenceMethod<Garcom>(repositorioGarcom.Cadastrar);
        // BuilderSetup.SetCreatePersistenceMethod<IList<Garcom>>((garcons) =>
        // {
        //     foreach (Garcom g in garcons)
        //         repositorioGarcom.Cadastrar(g);
        // });

        // // Produto
        // repositorioProduto = new RepositorioProduto(dbContext);

        // BuilderSetup.SetCreatePersistenceMethod<Produto>(repositorioProduto.Cadastrar);
        // BuilderSetup.SetCreatePersistenceMethod<IList<Produto>>((produtos) =>
        // {
        //     foreach (Produto p in produtos)
        //         repositorioProduto.Cadastrar(p);
        // });

        // // Conta
        // repositorioConta = new RepositorioConta(dbContext);

        // BuilderSetup.SetCreatePersistenceMethod<Conta>(repositorioConta.Cadastrar);
        // BuilderSetup.SetCreatePersistenceMethod<IList<Conta>>((contas) =>
        // {
        //     foreach (Conta c in contas)
        //         repositorioConta.Cadastrar(c);
        // });

        // // Pedido
        // repositorioPedido = new RepositorioPedido(dbContext);

        // BuilderSetup.SetCreatePersistenceMethod<Pedido>(repositorioPedido.Cadastrar);
        // BuilderSetup.SetCreatePersistenceMethod<IList<Pedido>>((pedidos) =>
        // {
        //     foreach (Pedido p in pedidos)
        //         repositorioPedido.Cadastrar(p);
        // });
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
