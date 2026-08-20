using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Testes.Integracao.Compartilhado.Orm;
using FizzWare.NBuilder;

namespace ControleDeBar.Testes.Integracao.Modulos.ModuloMesa;

[TestClass]
public class RepositorioMesaTests : RepositorioBaseTests
{
    [TestMethod]
    public void Cadastrar_ComTodosOsCampos_RegistraMesa()
    {
        // arrange
        Mesa mesa = Builder<Mesa>
            .CreateNew()
            .With(m => m.Identificacao = "Mesa01")
            .With(m => m.QuantidadeLugar = 4)
            .With(m => m.UserId = Guid.Empty)
            .Build();

        repositorioMesa.Cadastrar(mesa);

        dbContext.ChangeTracker.Clear();

        // act
        Mesa? mesaSelecionada = repositorioMesa.SelecionarPorId(mesa.Id);

        // assert
        Assert.IsNotNull(mesaSelecionada);
        Assert.AreEqual("Mesa01", mesaSelecionada.Identificacao);
        Assert.AreEqual(4, mesaSelecionada.QuantidadeLugar);
        Assert.AreEqual(StatusMesa.Livre, mesaSelecionada.StatusMesa);
    }

    [TestMethod]
    public void Editar_ComDadosValidos_AtualizaMesa()
    {
        // arrange
        Mesa mesa = Builder<Mesa>
            .CreateNew()
            .With(m => m.Identificacao = "Mesa03")
            .With(m => m.QuantidadeLugar = 4)
            .With(m => m.UserId = Guid.Empty)
            .Build();

        repositorioMesa.Cadastrar(mesa);
        dbContext.ChangeTracker.Clear();

        // act
        mesa.Identificacao = "Mesa03-Editada";
        mesa.QuantidadeLugar = 6;
        repositorioMesa.Editar(mesa.Id, mesa);

        dbContext.ChangeTracker.Clear();

        Mesa? mesaSelecionada = repositorioMesa.SelecionarPorId(mesa.Id);

        // assert
        Assert.IsNotNull(mesaSelecionada);
        Assert.AreEqual("Mesa03-Editada", mesaSelecionada.Identificacao);
        Assert.AreEqual(6, mesaSelecionada.QuantidadeLugar);
    }

    [TestMethod]
    public void Visualizar_MesaCadastrada_RetornaMesa()
    {
        // arrange
        Mesa mesa = Builder<Mesa>
            .CreateNew()
            .With(m => m.Identificacao = "Mesa06")
            .With(m => m.QuantidadeLugar = 4)
            .With(m => m.UserId = Guid.Empty)
            .Build();

        repositorioMesa.Cadastrar(mesa);
        dbContext.ChangeTracker.Clear();

        // act
        Mesa? mesaSelecionada = repositorioMesa.SelecionarPorId(mesa.Id);

        // assert
        Assert.IsNotNull(mesaSelecionada);
        Assert.AreEqual("Mesa06", mesaSelecionada.Identificacao);
    }

    [TestMethod]
    public void Listar_TodasAsMesas_RetornaTodas()
    {
        // arrange
        var mesas = Builder<Mesa>.CreateListOfSize(3)
            .All()
            .With(m => m.UserId = Guid.Empty)
            .Build();

        foreach (var mesa in mesas)
            repositorioMesa.Cadastrar(mesa);

        dbContext.ChangeTracker.Clear();

        // act
        var mesasSelecionadas = repositorioMesa.SelecionarTodos();

        // assert
        Assert.AreEqual(3, mesasSelecionadas.Count);
    }

    [TestMethod]
    public void Excluir_MesaLivreSemContaVinculada_RemoveMesa()
    {
        // arrange
        Mesa mesa = Builder<Mesa>
            .CreateNew()
            .With(m => m.Identificacao = "Mesa07")
            .With(m => m.QuantidadeLugar = 4)
            .With(m => m.UserId = Guid.Empty)
            .Build();

        repositorioMesa.Cadastrar(mesa);
        dbContext.ChangeTracker.Clear();

        // act
        bool conseguiuExcluir = repositorioMesa.Excluir(mesa.Id);

        dbContext.ChangeTracker.Clear();

        Mesa? mesaSelecionada = repositorioMesa.SelecionarPorId(mesa.Id);

        // assert
        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(mesaSelecionada);
    }

}