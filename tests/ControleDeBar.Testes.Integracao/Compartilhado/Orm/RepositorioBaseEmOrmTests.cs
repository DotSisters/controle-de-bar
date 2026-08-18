using ControleDeBar.Infra.Compartilhado.Orm;
using ControleDeBar.Testes.Integracao.Compartilhado.Identity;
using FizzWare.NBuilder;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Testes.Integracao.Compartilhado.Orm;

public abstract class RepositorioBaseEmOrmTests
{
    protected ControleDeBarDbContext dbContext = null!;
    // protected RepositorioMesaEmOrm repositorioMesa = null!;
    // protected RepositorioGarcomEmOrm repositorioGarcom = null!;
    // protected RepositorioProdutoEmOrm repositorioProduto = null!;
    // protected RepositorioContaEmOrm repositorioConta = null!;
    // protected RepositorioPedidoEmOrm repositorioPedido = null!;

    [TestInitialize]
    public void InicializarContexto()
    {
        dbContext = CriarDbContext(Guid.NewGuid());

        // Mesa
        // repositorioMesa = new RepositorioMesaEmOrm(dbContext);

        // BuilderSetup.SetCreatePersistenceMethod<Mesa>(repositorioMesa.Cadastrar);
        // BuilderSetup.SetCreatePersistenceMethod<IList<Mesa>>((mesas) =>
        // {
        //     foreach (Mesa m in mesas)
        //         repositorioMesa.Cadastrar(m);
        // });

        // // Garcom
        // repositorioGarcom = new RepositorioGarcomEmOrm(dbContext);

        // BuilderSetup.SetCreatePersistenceMethod<Garcom>(repositorioGarcom.Cadastrar);
        // BuilderSetup.SetCreatePersistenceMethod<IList<Garcom>>((garcons) =>
        // {
        //     foreach (Garcom g in garcons)
        //         repositorioGarcom.Cadastrar(g);
        // });

        // // Produto
        // repositorioProduto = new RepositorioProdutoEmOrm(dbContext);

        // BuilderSetup.SetCreatePersistenceMethod<Produto>(repositorioProduto.Cadastrar);
        // BuilderSetup.SetCreatePersistenceMethod<IList<Produto>>((produtos) =>
        // {
        //     foreach (Produto p in produtos)
        //         repositorioProduto.Cadastrar(p);
        // });

        // // Conta
        // repositorioConta = new RepositorioContaEmOrm(dbContext);

        // BuilderSetup.SetCreatePersistenceMethod<Conta>(repositorioConta.Cadastrar);
        // BuilderSetup.SetCreatePersistenceMethod<IList<Conta>>((contas) =>
        // {
        //     foreach (Conta c in contas)
        //         repositorioConta.Cadastrar(c);
        // });

        // // Pedido
        // repositorioPedido = new RepositorioPedidoEmOrm(dbContext);

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