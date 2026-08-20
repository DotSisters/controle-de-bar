using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Testes.Integracao.Compartilhado.Orm;
using FizzWare.NBuilder;
namespace ControleDeBar.Testes.Integracao.Modulos.ModuloGarcom;

[TestClass]
public class RepositorioGarcomTests : RepositorioBaseTests
{
    [TestMethod]
    public void Cadastrar_ComTodosOsCampos_DevePersistirGarcom()
    {
        Garcom garcom = Builder<Garcom>
            .CreateNew()
            .With(g => g.Nome = "Teste")
            .With(g => g.Telefone = "(11) 11111-1111")
            .With(g => g.Cpf = "111.111.111-11")
            .With(g => g.UserId = Guid.Empty)
            .Build();

        repositorioGarcom.Cadastrar(garcom);

        dbContext.ChangeTracker.Clear();

        Garcom? garcomSelecionado = repositorioGarcom.SelecionarPorId(garcom.Id);

        Assert.IsNotNull(garcomSelecionado);
        Assert.AreEqual("Teste", garcomSelecionado.Nome);
        Assert.AreEqual("(11) 11111-1111", garcomSelecionado.Telefone);
        Assert.AreEqual("111.111.111-11", garcomSelecionado.Cpf);
    }

    [TestMethod]
    public void Editar_ComDadosValidos_AtualizaGarcom()
    {
        Garcom garcom = Builder<Garcom>
            .CreateNew()
            .With(g => g.Nome = "Teste")
            .With(g => g.Telefone = "(11) 11111-1111")
            .With(g => g.Cpf = "111.111.111-11")
            .With(g => g.UserId = Guid.Empty)
            .Build();

        repositorioGarcom.Cadastrar(garcom);
        dbContext.ChangeTracker.Clear();

        garcom.Nome = "Teste Editado";
        garcom.Telefone = "(22) 22222-2222";
        garcom.Cpf = "222.222.222-22";
        repositorioGarcom.Editar(garcom.Id, garcom);

        dbContext.ChangeTracker.Clear();

        Garcom? garcomSelecionado = repositorioGarcom.SelecionarPorId(garcom.Id);

        Assert.IsNotNull(garcomSelecionado);
        Assert.AreEqual("Teste Editado", garcomSelecionado.Nome);
        Assert.AreEqual("(22) 22222-2222", garcomSelecionado.Telefone);
        Assert.AreEqual("222.222.222-22", garcomSelecionado.Cpf);
    }

    [TestMethod]
    public void Visualizar_GarcomCadastrado_RetornaGarcom()
    {
        Garcom garcom = Builder<Garcom>
            .CreateNew()
            .With(g => g.Nome = "Teste")
            .With(g => g.Telefone = "(11) 11111-1111")
            .With(g => g.Cpf = "111.111.111-11")
            .With(g => g.UserId = Guid.Empty)
            .Build();

        repositorioGarcom.Cadastrar(garcom);
        dbContext.ChangeTracker.Clear();

        Garcom? garcomSelecionado = repositorioGarcom.SelecionarPorId(garcom.Id);

        Assert.IsNotNull(garcomSelecionado);
        Assert.AreEqual("Teste", garcomSelecionado.Nome);
        Assert.AreEqual("(11) 11111-1111", garcomSelecionado.Telefone);
        Assert.AreEqual("111.111.111-11", garcomSelecionado.Cpf);
    }

    [TestMethod]
    public void Listar_TodosOsGarcons_RetornaTodos()
    {
        var garcons = Builder<Garcom>.CreateListOfSize(3)
            .All()
            .With(g => g.UserId = Guid.Empty)
            .Build();

        foreach (var garcom in garcons)
            repositorioGarcom.Cadastrar(garcom);

        dbContext.ChangeTracker.Clear();

        var garconsSelecionados = repositorioGarcom.SelecionarTodos();

        Assert.AreEqual(3, garconsSelecionados.Count);
    }

    [TestMethod]
    public void Excluir_SemContaVinculada_RemoveGarcom()
    {
        Garcom garcom = Builder<Garcom>
            .CreateNew()
            .With(g => g.Nome = "Teste")
            .With(g => g.Telefone = "(11) 11111-1111")
            .With(g => g.Cpf = "111.111.111-11")
            .With(g => g.UserId = Guid.Empty)
            .Build();

        repositorioGarcom.Cadastrar(garcom);
        dbContext.ChangeTracker.Clear();

        bool conseguiuExcluir = repositorioGarcom.Excluir(garcom.Id);

        dbContext.ChangeTracker.Clear();

        Garcom? garcomSelecionado = repositorioGarcom.SelecionarPorId(garcom.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(garcomSelecionado);
    }

}
