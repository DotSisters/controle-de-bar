using ControleDeBar.Aplicacao.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using FluentResults;
using Moq;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloGarcom;

[TestClass]
public sealed class ServicoGarcomTests
{
    [TestMethod]
    public void Cadastrar_ComTodosCampos_PersisteGarcom()
    {
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioConta> repositorioConta = new();

        repositorioGarcom.Setup(r => r.SelecionarTodos()).Returns([]);

        Garcom? garcomCadastrado = null;

        repositorioGarcom
            .Setup(r => r.Cadastrar(It.IsAny<Garcom>()))
            .Callback<Garcom>(
                garcom => garcomCadastrado = garcom
            );

        ServicoGarcom servicoGarcom = new ServicoGarcom(
            repositorioGarcom.Object,
            repositorioConta.Object
        );

        Result resultado = servicoGarcom.Cadastrar(new CadastrarGarcomDto(
            "Teste",
            "(11) 11111-1111",
            "111.111.111-11"
        ));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(garcomCadastrado);
        Assert.AreEqual("Teste", garcomCadastrado.Nome);
        Assert.AreEqual("(11) 11111-1111", garcomCadastrado.Telefone);
        Assert.AreEqual("111.111.111-11", garcomCadastrado.Cpf);

        repositorioGarcom.Verify(r => r.Cadastrar(It.IsAny<Garcom>()), Times.Once);
    }

    [TestMethod]
    public void Cadastrar_ComNomeVazio_RetornaErro()
    {
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioConta> repositorioConta = new();

        repositorioGarcom.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoGarcom servicoGarcom = new(
            repositorioGarcom.Object,
            repositorioConta.Object
        );

        Result resultado = servicoGarcom.Cadastrar(new CadastrarGarcomDto(
            string.Empty,
            "(11) 11111-1111",
            "111.111.111-11"
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("O campo \"Nome\" deve ser preenchido.", resultado.Errors.First().Message);

        repositorioGarcom.Verify(r => r.Cadastrar(It.IsAny<Garcom>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_ComTelefoneVazio_RetornaErro()
    {
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioConta> repositorioConta = new();

        repositorioGarcom.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoGarcom servicoGarcom = new(
            repositorioGarcom.Object,
            repositorioConta.Object
        );

        Result resultado = servicoGarcom.Cadastrar(new CadastrarGarcomDto(
            "Teste",
            string.Empty,
            "111.111.111-11"
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("O campo \"Telefone\" deve ser preenchido.", resultado.Errors.First().Message);

        repositorioGarcom.Verify(r => r.Cadastrar(It.IsAny<Garcom>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_ComCpfVazio_RetornaErro()
    {
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioConta> repositorioConta = new();

        repositorioGarcom.Setup(r => r.SelecionarTodos()).Returns([]);

        ServicoGarcom servicoGarcom = new(
            repositorioGarcom.Object,
            repositorioConta.Object
        );

        Result resultado = servicoGarcom.Cadastrar(new CadastrarGarcomDto(
            "Teste",
            "(11) 11111-1111",
            string.Empty

        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("O campo \"Cpf\" deve ser preenchido.", resultado.Errors.First().Message);

        repositorioGarcom.Verify(r => r.Cadastrar(It.IsAny<Garcom>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_TelefoneDuplicado_RetornaFalha()
    {
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioConta> repositorioConta = new();

        repositorioGarcom.Setup(r => r.SelecionarTodos())
        .Returns([new Garcom(
            "Teste 1",
            "(11) 11111-1111",
            "111.111.111-11"
        )]);

        ServicoGarcom servicoGarcom = new(
            repositorioGarcom.Object,
            repositorioConta.Object
        );

        Result resultado = servicoGarcom.Cadastrar(new CadastrarGarcomDto(
            "Teste 2",
            "(11) 11111-1111",
            "222.222.222-22"
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Telefone", resultado.Errors.Single().Metadata["Campo"]);
        Assert.Contains("Já existe", resultado.Errors.Single().Message);

        repositorioGarcom.Verify(r => r.Cadastrar(It.IsAny<Garcom>()), Times.Never);
    }

    [TestMethod]
    public void Cadastrar_CpfDuplicado_RetornaFalha()
    {
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioConta> repositorioConta = new();

        repositorioGarcom.Setup(r => r.SelecionarTodos())
        .Returns([new Garcom(
            "Teste 1",
            "(11) 11111-1111",
            "111.111.111-11"
        )]);

        ServicoGarcom servicoGarcom = new(
            repositorioGarcom.Object,
            repositorioConta.Object
        );

        Result resultado = servicoGarcom.Cadastrar(new CadastrarGarcomDto(
            "Teste 2",
            "(22) 22222-222",
            "111.111.111-11"
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Cpf", resultado.Errors.Single().Metadata["Campo"]);
        Assert.Contains("Já existe", resultado.Errors.Single().Message);

        repositorioGarcom.Verify(r => r.Cadastrar(It.IsAny<Garcom>()), Times.Never);
    }

    [TestMethod]
    public void Editar_ComDadosValidos_PersisteGarcom()
    {
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioConta> repositorioConta = new();

        Garcom garcomExistente = new Garcom(
            "Teste",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        List<Garcom> garcom = new() { garcomExistente };

        repositorioGarcom.Setup(r => r.SelecionarTodos()).Returns(() => garcom);
        repositorioGarcom
            .Setup(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Garcom>()))
            .Callback<Guid, Garcom>((id, garcomAtualizado) =>
            {
                garcomAtualizado.Id = id;
                int index = garcom.FindIndex(g => g.Id == id);
                if (index >= 0)
                    garcom[index].Atualizar(garcomAtualizado);
            })
            .Returns<Guid, Garcom>((id, garcomAtualizado) => garcom.Any(g => g.Id == id));

        ServicoGarcom servicoGarcom = new ServicoGarcom(
            repositorioGarcom.Object,
            repositorioConta.Object
        );

        Result resultado = servicoGarcom.Editar(new EditarGarcomDto(
            garcomExistente.Id,
            "Teste 2",
            "(22) 22222-2222",
            "222.222.222-22"
        ));

        Assert.IsTrue(resultado.IsSuccess);
        repositorioGarcom.Verify(r => r.Editar(garcomExistente.Id, It.IsAny<Garcom>()), Times.Once);

        List<ListarGarconsDto> garconsListados = servicoGarcom.SelecionarTodos();

        Assert.HasCount(1, garconsListados);
        Assert.AreEqual("Teste 2", garconsListados[0].Nome);
        Assert.AreEqual("(22) 22222-2222", garconsListados[0].Telefone);
        Assert.AreEqual("222.222.222-22", garconsListados[0].Cpf);
    }

    [TestMethod]
    public void Editar_ComTelefoneDuplicado_RetornaFalha()
    {
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioConta> repositorioConta = new();

        Garcom garcomExistente = new(
            "Teste",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        Garcom outroGarcom = new(
            "Teste 2",
            "(22) 22222-2222",
            "222.222.222-22"
        );

        List<Garcom> garcoms = new() { garcomExistente, outroGarcom };

        repositorioGarcom.Setup(r => r.SelecionarTodos()).Returns(() => garcoms);

        ServicoGarcom servicoGarcom = new ServicoGarcom(
            repositorioGarcom.Object,
            repositorioConta.Object
        );

        Result resultado = servicoGarcom.Editar(new EditarGarcomDto(
            garcomExistente.Id,
            "Teste",
            "(22) 22222-2222",
            "111.111.111-11"
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Telefone", resultado.Errors.Single().Metadata["Campo"]);
        Assert.Contains("Já existe", resultado.Errors.Single().Message);
        repositorioGarcom.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Garcom>()), Times.Never);
    }

    [TestMethod]
    public void Editar_ComCpfDuplicado_RetornaFalha()
    {
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioConta> repositorioConta = new();

        Garcom garcomExistente = new(
            "Teste",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        Garcom outroGarcom = new(
            "Teste 2",
            "(22) 22222-2222",
            "222.222.222-22"
        );

        List<Garcom> garcoms = new() { garcomExistente, outroGarcom };

        repositorioGarcom.Setup(r => r.SelecionarTodos()).Returns(() => garcoms);

        ServicoGarcom servicoGarcom = new ServicoGarcom(
            repositorioGarcom.Object,
            repositorioConta.Object
        );

        Result resultado = servicoGarcom.Editar(new EditarGarcomDto(
            garcomExistente.Id,
            "Teste",
            "(11) 11111-1111",
            "222.222.222-22"
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual("Cpf", resultado.Errors.Single().Metadata["Campo"]);
        Assert.Contains("Já existe", resultado.Errors.Single().Message);
        repositorioGarcom.Verify(r => r.Editar(It.IsAny<Guid>(), It.IsAny<Garcom>()), Times.Never);
    }

    [TestMethod]
    public void SelecionarPorId_RetornaGarcom()
    {
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioConta> repositorioConta = new();

        Garcom garcomExistente = new Garcom(
            "Teste",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        repositorioGarcom
            .Setup(r => r.SelecionarPorId(garcomExistente.Id))
            .Returns(garcomExistente);

        ServicoGarcom servicoGarcom = new ServicoGarcom(
            repositorioGarcom.Object,
            repositorioConta.Object
        );

        Result<DetalhesGarcomDto> resultado = servicoGarcom.SelecionarPorId(garcomExistente.Id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(resultado.Value);
        Assert.AreEqual(garcomExistente.Id, resultado.Value.Id);
        Assert.AreEqual("Teste", resultado.Value.Nome);
        Assert.AreEqual("(11) 11111-1111", resultado.Value.Telefone);
        Assert.AreEqual("111.111.111-11", resultado.Value.Cpf);

    }

    [TestMethod]
    public void SelecionarTodos_RetornaGarconsCadastrados()
    {
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioConta> repositorioConta = new();

        List<Garcom> garcons = new()
        {
            new Garcom(
                "Teste",
                "(11) 11111-1111",
                "111.111.111-11"
            ),
            new Garcom(
                "Teste 2",
                "(22) 22222-2222",
                "222.222.222-22"
            )
        };

        repositorioGarcom.Setup(r => r.SelecionarTodos()).Returns(() => garcons);

        ServicoGarcom servicoGarcom = new ServicoGarcom(
            repositorioGarcom.Object,
            repositorioConta.Object
        );

        List<ListarGarconsDto> garconsListados = servicoGarcom.SelecionarTodos();

        Assert.HasCount(2, garconsListados);
        Assert.AreEqual("Teste", garconsListados[0].Nome);
        Assert.AreEqual("(11) 11111-1111", garconsListados[0].Telefone);
        Assert.AreEqual("111.111.111-11", garconsListados[0].Cpf);


        Assert.AreEqual("Teste 2", garconsListados[1].Nome);
        Assert.AreEqual("(22) 22222-2222", garconsListados[1].Telefone);
        Assert.AreEqual("222.222.222-22", garconsListados[1].Cpf);
    }

    [TestMethod]
    public void Excluir_SemContasVinculadas_ExcluiGarcom()
    {
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioConta> repositorioConta = new();

        Garcom garcom = new Garcom(
            "Teste",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        repositorioGarcom
            .Setup(r => r.SelecionarPorId(garcom.Id))
            .Returns(garcom);
        repositorioConta
            .Setup(r => r.SelecionarTodos())
            .Returns(new List<Conta>());

        ServicoGarcom servicoGarcom = new ServicoGarcom(
            repositorioGarcom.Object,
            repositorioConta.Object
        );

        Result resultado = servicoGarcom.Excluir(garcom.Id);

        Assert.IsTrue(resultado.IsSuccess);
        repositorioGarcom.Verify(r => r.Excluir(garcom.Id), Times.Once);
    }

    [TestMethod]
    public void Excluir_ComVinculo_ContaEmAberto_RetornaFalha()
    {
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioConta> repositorioConta = new();
        Garcom garcom = new Garcom(
            "Teste",
            "(11) 11111-1111",
            "111.111.111-11"
        );
        repositorioGarcom
            .Setup(r => r.SelecionarPorId(garcom.Id))
            .Returns(garcom);
        repositorioConta
            .Setup(r => r.ExisteContaAbertaPorGarcom(garcom.Id))
            .Returns(true);
        ServicoGarcom servicoGarcom = new ServicoGarcom(
            repositorioGarcom.Object,
            repositorioConta.Object
        );
        Result resultado = servicoGarcom.Excluir(garcom.Id);
        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains("vinculado a uma conta em aberto", resultado.Errors.Single().Message);
        repositorioGarcom.Verify(r => r.Excluir(garcom.Id), Times.Never);
    }

    [TestMethod]
    public void Excluir_ComVinculo_ContaFechada_ExcluiGarcom()
    {
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioConta> repositorioConta = new();

        Garcom garcom = new Garcom(
            "Teste",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        repositorioGarcom
            .Setup(r => r.SelecionarPorId(garcom.Id))
            .Returns(garcom);
        repositorioConta
            .Setup(r => r.ExisteContaAbertaPorGarcom(garcom.Id))
            .Returns(false);

        ServicoGarcom servicoGarcom = new ServicoGarcom(
            repositorioGarcom.Object,
            repositorioConta.Object
        );

        Result resultado = servicoGarcom.Excluir(garcom.Id);

        Assert.IsTrue(resultado.IsSuccess);
        repositorioGarcom.Verify(r => r.Excluir(garcom.Id), Times.Once);
    }
}
