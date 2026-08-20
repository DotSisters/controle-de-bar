using ControleDeBar.Aplicacao.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using FluentResults;
using Moq;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloMesa;

[TestClass]
public sealed class ServicoMesaTests
{
    [TestMethod]
    public void Cadastrar_ComDadosValidos_PersisteMesa()
    {
        Mock<IRepositorioMesa> repositorioMesa = new();
        repositorioMesa.Setup(r => r.SelecionarTodos()).Returns([]);

        Mesa? mesaCadastrada = null;

        repositorioMesa
            .Setup(r => r.Cadastrar(It.IsAny<Mesa>()))
            .Callback<Mesa>(mesa => mesaCadastrada = mesa);

        ServicoMesa servicoMesa = new(repositorioMesa.Object);

        Result resultado = servicoMesa.Cadastrar(
            new CadastrarMesaDto("Mesa01", 10, StatusMesa.Livre)
        );

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(mesaCadastrada);
        Assert.AreEqual("Mesa01", mesaCadastrada.Identificacao);

        repositorioMesa.Verify(
            r => r.Cadastrar(It.IsAny<Mesa>()),
            Times.Once
        );
    }

    [TestMethod]
    public void Cadastrar_ComIdentificacaoVazia_RetornaErro()
    {
        Mock<IRepositorioMesa> repositorioMesa = new();
        repositorioMesa.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoMesa servicoMesa = new(repositorioMesa.Object);

        Result resultado = servicoMesa.Cadastrar(
            new CadastrarMesaDto(string.Empty, 10, StatusMesa.Livre)
        );

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "O campo \"Identificação\" deve ser preenchido.",
            resultado.Errors.First().Message
        );

        repositorioMesa.Verify(
            r => r.Cadastrar(It.IsAny<Mesa>()),
            Times.Never
        );
    }

    [TestMethod]
    public void Cadastrar_ComQuantidadeLugarInvalida_RetornaErro()
    {
        Mock<IRepositorioMesa> repositorioMesa = new();
        repositorioMesa.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoMesa servicoMesa = new(repositorioMesa.Object);

        Result resultado = servicoMesa.Cadastrar(
            new CadastrarMesaDto("Mesa01", 0, StatusMesa.Livre)
        );

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "O campo \"Quantidade de Lugares\" deve ser maior que zero.",
            resultado.Errors.First().Message
        );

        repositorioMesa.Verify(
            r => r.Cadastrar(It.IsAny<Mesa>()),
            Times.Never
        );
    }

    [TestMethod]
    public void Cadastrar_ComStatusInicialOcupada_RetornaErro()
    {
        Mock<IRepositorioMesa> repositorioMesa = new();
        repositorioMesa.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoMesa servicoMesa = new(repositorioMesa.Object);

        Result resultado = servicoMesa.Cadastrar(
            new CadastrarMesaDto("Mesa01", 4, StatusMesa.Ocupada)
        );

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Uma mesa cadastrada deve iniciar com o status Livre.",
            resultado.Errors.First().Message
        );

        repositorioMesa.Verify(
            r => r.Cadastrar(It.IsAny<Mesa>()),
            Times.Never
        );
    }

    [TestMethod]
    public void Cadastrar_ComNumeroDuplicado_RetornaErro()
    {
        Mock<IRepositorioMesa> repositorioMesa = new();

        repositorioMesa
            .Setup(r => r.SelecionarTodos())
            .Returns([new Mesa("Mesa01", 4)]);

        ServicoMesa servicoMesa = new(repositorioMesa.Object);

        Result resultado = servicoMesa.Cadastrar(
            new CadastrarMesaDto("Mesa01", 4, StatusMesa.Livre)
        );

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Já existe uma mesa com este número.",
            resultado.Errors.First().Message
        );

        repositorioMesa.Verify(
            r => r.Cadastrar(It.IsAny<Mesa>()),
            Times.Never
        );
    }

    [TestMethod]
    public void Editar_ComDadosValidos_RetornaSucesso()
    {
        Mock<IRepositorioMesa> repositorioMesa = new();

        repositorioMesa
            .Setup(r => r.SelecionarTodos())
            .Returns([]);

        repositorioMesa
            .Setup(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Mesa>()))
            .Returns(true);

        ServicoMesa servicoMesa = new(repositorioMesa.Object);

        Result resultado = servicoMesa.Editar(
            new EditarMesaDto(
                Guid.NewGuid(),
                "Mesa02",
                6,
                StatusMesa.Livre
            )
        );

        Assert.IsTrue(resultado.IsSuccess);

        repositorioMesa.Verify(
            r => r.Editar(It.IsAny<Guid>(), It.IsAny<Mesa>()),
            Times.Once
        );
    }

    [TestMethod]
    public void Editar_ParaNumeroJaUtilizado_RetornaErro()
    {
        Guid idMesaExistente = Guid.NewGuid();

        Mock<IRepositorioMesa> repositorioMesa = new();

        repositorioMesa
            .Setup(r => r.SelecionarTodos())
            .Returns([
                new Mesa("Mesa01", 4)
                {
                    Id = idMesaExistente
                },
                new Mesa("Mesa02", 4)
            ]);

        ServicoMesa servicoMesa = new(repositorioMesa.Object);

        Result resultado = servicoMesa.Editar(
            new EditarMesaDto(
                idMesaExistente,
                "Mesa02",
                6,
                StatusMesa.Livre
            )
        );

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Já existe uma mesa com este número.",
            resultado.Errors.First().Message
        );

        repositorioMesa.Verify(
            r => r.Editar(It.IsAny<Guid>(), It.IsAny<Mesa>()),
            Times.Never
        );
    }

    [TestMethod]
    public void SelecionarPorId_MesaCadastrada_RetornaSucesso()
    {
        Mesa mesaExistente = new("Mesa01", 4);

        Mock<IRepositorioMesa> repositorioMesa = new();

        repositorioMesa
            .Setup(r => r.SelecionarPorId(mesaExistente.Id))
            .Returns(mesaExistente);

        ServicoMesa servicoMesa = new(repositorioMesa.Object);

        Result<DetalhesMesaDto> resultado =
            servicoMesa.SelecionarPorId(mesaExistente.Id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual("Mesa01", resultado.Value.Identificacao);
    }

    [TestMethod]
    public void SelecionarTodos_ComMesasCadastradas_RetornaTodas()
    {
        Mock<IRepositorioMesa> repositorioMesa = new();

        repositorioMesa
            .Setup(r => r.SelecionarTodos())
            .Returns([
                new Mesa("Mesa01", 4),
                new Mesa("Mesa02", 6)
            ]);

        ServicoMesa servicoMesa = new(repositorioMesa.Object);

        List<ListarMesasDto> resultado = servicoMesa.SelecionarTodos();

        Assert.AreEqual(2, resultado.Count);
    }

    [TestMethod]
    public void Excluir_MesaExistente_RetornaSucesso()
    {
        Mesa mesaExistente = new("Mesa01", 4);

        Mock<IRepositorioMesa> repositorioMesa = new();

        repositorioMesa
            .Setup(r => r.SelecionarPorId(mesaExistente.Id))
            .Returns(mesaExistente);

        repositorioMesa
            .Setup(r => r.Excluir(mesaExistente.Id))
            .Returns(true);

        ServicoMesa servicoMesa = new(repositorioMesa.Object);

        Result resultado = servicoMesa.Excluir(mesaExistente.Id);

        Assert.IsTrue(resultado.IsSuccess);

        repositorioMesa.Verify(
            r => r.Excluir(mesaExistente.Id),
            Times.Once
        );
    }

    [TestMethod]
    public void Excluir_MesaVinculadaContaEmAberto_RetornaErro()
    {
        Mock<IRepositorioMesa> repositorioMesa = new();

        Mesa mesaExistente = new("Mesa01", 4);
        mesaExistente.MarcarComoOcupada();

        repositorioMesa
            .Setup(r => r.SelecionarPorId(mesaExistente.Id))
            .Returns(mesaExistente);

        ServicoMesa servicoMesa = new(repositorioMesa.Object);

        Result resultado = servicoMesa.Excluir(mesaExistente.Id);

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Não é possível excluir uma mesa vinculada a uma conta em aberto.",
            resultado.Errors.First().Message
        );

        repositorioMesa.Verify(
            r => r.Excluir(It.IsAny<Guid>()),
            Times.Never
        );
    }

    [TestMethod]
    public void Excluir_MesaNaoEncontrada_RetornaErro()
    {
        Mock<IRepositorioMesa> repositorioMesa = new();

        repositorioMesa
            .Setup(r => r.SelecionarPorId(It.IsAny<Guid>()))
            .Returns((Mesa?)null);

        ServicoMesa servicoMesa = new(repositorioMesa.Object);

        Result resultado = servicoMesa.Excluir(Guid.NewGuid());

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Mesa não encontrada.",
            resultado.Errors.First().Message
        );

        repositorioMesa.Verify(
            r => r.Excluir(It.IsAny<Guid>()),
            Times.Never
        );
    }

    [TestMethod]
    public void Excluir_QuandoRepositorioFalha_RetornaErro()
    {
        Mesa mesa = new("Mesa01", 4);

        Mock<IRepositorioMesa> repositorioMesa = new();

        repositorioMesa
            .Setup(r => r.SelecionarPorId(mesa.Id))
            .Returns(mesa);

        repositorioMesa
            .Setup(r => r.Excluir(mesa.Id))
            .Returns(false);

        ServicoMesa servicoMesa = new(repositorioMesa.Object);

        Result resultado = servicoMesa.Excluir(mesa.Id);

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Não foi possível excluir a mesa.",
            resultado.Errors.First().Message
        );
    }
}