using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Infra.Modulos.ModuloConta;
using ControleDeBar.Testes.Integracao.Compartilhado.Orm;
using FizzWare.NBuilder;

namespace ControleDeBar.Testes.Integracao.Modulos.ModuloConta;

[TestClass]
public class RepositorioContaTests : RepositorioBaseTests
{
    [TestMethod]
    public void Cadastrar_ComTodosOsCampos_DevePersistirConta()
    {
        Mesa mesa = Builder<Mesa>
            .CreateNew()
            .With(m => m.Identificacao = "Mesa 01")
            .With(m => m.QuantidadeLugar = 4)
            .With(m => m.UserId = Guid.Empty)
            .Build();

        Garcom garcom = Builder<Garcom>
            .CreateNew()
            .With(g => g.Nome = "João")
            .With(g => g.Telefone = "(11) 11111-1111")
            .With(g => g.Cpf = "111.111.111-11")
            .With(g => g.UserId = Guid.Empty)
            .Build();

        repositorioMesa.Cadastrar(mesa);
        repositorioGarcom.Cadastrar(garcom);

        Conta conta = new Conta(
            "Cliente Teste",
            mesa.Id,
            mesa.Identificacao,
            garcom.Id,
            garcom.Nome
        );

        conta.UserId = Guid.Empty;

        repositorioConta.Cadastrar(conta);

        dbContext.ChangeTracker.Clear();

        Conta? contaSelecionada =
            repositorioConta.SelecionarPorId(conta.Id);

        Assert.IsNotNull(contaSelecionada);
        Assert.AreEqual("Cliente Teste", contaSelecionada.NomeCliente);
        Assert.AreEqual(mesa.Id, contaSelecionada.MesaId);
        Assert.AreEqual("Mesa 01", contaSelecionada.IdentificacaoMesa);
        Assert.AreEqual(garcom.Id, contaSelecionada.GarcomId);
        Assert.AreEqual("João", contaSelecionada.NomeGarcom);
        Assert.AreEqual(SituacaoConta.Aberta, contaSelecionada.Situacao);
    }

    [TestMethod]
    public void Editar_ComDadosValidos_AtualizaConta()
    {
        Mesa mesa = new Mesa("Mesa 01", 4);
        Garcom garcom = new Garcom(
            "João",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        repositorioMesa.Cadastrar(mesa);
        repositorioGarcom.Cadastrar(garcom);

        Conta conta = new Conta(
            "Cliente Teste",
            mesa.Id,
            mesa.Identificacao,
            garcom.Id,
            garcom.Nome
        );

        repositorioConta.Cadastrar(conta);
        dbContext.ChangeTracker.Clear();

        Conta contaAtualizada = new Conta(
            "Cliente Alterado",
            mesa.Id,
            mesa.Identificacao,
            garcom.Id,
            garcom.Nome
        );

        bool conseguiuEditar =
            repositorioConta.Editar(conta.Id, contaAtualizada);

        dbContext.ChangeTracker.Clear();

        Conta? contaSelecionada =
            repositorioConta.SelecionarPorId(conta.Id);

        Assert.IsTrue(conseguiuEditar);
        Assert.IsNotNull(contaSelecionada);
        Assert.AreEqual(
            "Cliente Alterado",
            contaSelecionada.NomeCliente
        );
    }

    [TestMethod]
    public void SelecionarPorId_ContaCadastrada_RetornaContaComRelacionamentos()
    {
        Mesa mesa = new Mesa("Mesa 01", 4);
        Garcom garcom = new Garcom(
            "João",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        repositorioMesa.Cadastrar(mesa);
        repositorioGarcom.Cadastrar(garcom);

        Conta conta = new Conta(
            "Cliente Teste",
            mesa.Id,
            mesa.Identificacao,
            garcom.Id,
            garcom.Nome
        );

        repositorioConta.Cadastrar(conta);

        dbContext.ChangeTracker.Clear();

        Conta? contaSelecionada =
            repositorioConta.SelecionarPorId(conta.Id);

        Assert.IsNotNull(contaSelecionada);
        Assert.IsNotNull(contaSelecionada.Mesa);
        Assert.IsNotNull(contaSelecionada.Garcom);
        Assert.AreEqual("Mesa 01", contaSelecionada.Mesa.Identificacao);
        Assert.AreEqual("João", contaSelecionada.Garcom.Nome);
    }

    [TestMethod]
    public void SelecionarPorId_ContaNaoCadastrada_RetornaNulo()
    {
        Conta? contaSelecionada =
            repositorioConta.SelecionarPorId(Guid.NewGuid());

        Assert.IsNull(contaSelecionada);
    }

    [TestMethod]
    public void SelecionarTodos_ComContasCadastradas_RetornaTodas()
    {
        Mesa primeiraMesa = new Mesa("Mesa 01", 4);
        Mesa segundaMesa = new Mesa("Mesa 02", 4);

        Garcom primeiroGarcom = new Garcom(
            "João",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        Garcom segundoGarcom = new Garcom(
            "Maria",
            "(22) 22222-2222",
            "222.222.222-22"
        );

        repositorioMesa.Cadastrar(primeiraMesa);
        repositorioMesa.Cadastrar(segundaMesa);
        repositorioGarcom.Cadastrar(primeiroGarcom);
        repositorioGarcom.Cadastrar(segundoGarcom);

        Conta primeiraConta = new Conta(
            "Cliente 1",
            primeiraMesa.Id,
            primeiraMesa.Identificacao,
            primeiroGarcom.Id,
            primeiroGarcom.Nome
        );

        Conta segundaConta = new Conta(
            "Cliente 2",
            segundaMesa.Id,
            segundaMesa.Identificacao,
            segundoGarcom.Id,
            segundoGarcom.Nome
        );

        repositorioConta.Cadastrar(primeiraConta);
        repositorioConta.Cadastrar(segundaConta);

        dbContext.ChangeTracker.Clear();

        List<Conta> contas =
            repositorioConta.SelecionarTodos();

        Assert.AreEqual(2, contas.Count);
        Assert.IsTrue(
            contas.Any(conta => conta.NomeCliente == "Cliente 1")
        );
        Assert.IsTrue(
            contas.Any(conta => conta.NomeCliente == "Cliente 2")
        );
    }

    [TestMethod]
    public void ExisteContaAbertaPorMesa_ComContaAberta_RetornaVerdadeiro()
    {
        Mesa mesa = new Mesa("Mesa 01", 4);
        Garcom garcom = new Garcom(
            "João",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        repositorioMesa.Cadastrar(mesa);
        repositorioGarcom.Cadastrar(garcom);

        Conta conta = new Conta(
            "Cliente Teste",
            mesa.Id,
            mesa.Identificacao,
            garcom.Id,
            garcom.Nome
        );

        repositorioConta.Cadastrar(conta);
        dbContext.ChangeTracker.Clear();

        bool existeContaAberta =
            repositorioConta.ExisteContaAbertaPorMesa(mesa.Id);

        Assert.IsTrue(existeContaAberta);
    }

    [TestMethod]
    public void ExisteContaAbertaPorMesa_SemContaAberta_RetornaFalso()
    {
        Mesa mesa = new Mesa("Mesa 01", 4);
        Garcom garcom = new Garcom(
            "João",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        repositorioMesa.Cadastrar(mesa);
        repositorioGarcom.Cadastrar(garcom);

        Conta conta = new Conta(
            "Cliente Teste",
            mesa.Id,
            mesa.Identificacao,
            garcom.Id,
            garcom.Nome
        );

        conta.Fechar(garcom.Nome, mesa.Identificacao);

        repositorioConta.Cadastrar(conta);
        dbContext.ChangeTracker.Clear();

        bool existeContaAberta =
            repositorioConta.ExisteContaAbertaPorMesa(mesa.Id);

        Assert.IsFalse(existeContaAberta);
    }

    [TestMethod]
    public void ExisteContaAbertaPorGarcom_ComContaAberta_RetornaVerdadeiro()
    {
        Mesa mesa = new Mesa("Mesa 01", 4);
        Garcom garcom = new Garcom(
            "João",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        repositorioMesa.Cadastrar(mesa);
        repositorioGarcom.Cadastrar(garcom);

        Conta conta = new Conta(
            "Cliente Teste",
            mesa.Id,
            mesa.Identificacao,
            garcom.Id,
            garcom.Nome
        );

        repositorioConta.Cadastrar(conta);
        dbContext.ChangeTracker.Clear();

        bool existeContaAberta =
            repositorioConta.ExisteContaAbertaPorGarcom(garcom.Id);

        Assert.IsTrue(existeContaAberta);
    }

    [TestMethod]
    public void ExisteContaAbertaPorGarcom_SemContaAberta_RetornaFalso()
    {
        Mesa mesa = new Mesa("Mesa 01", 4);
        Garcom garcom = new Garcom(
            "João",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        repositorioMesa.Cadastrar(mesa);
        repositorioGarcom.Cadastrar(garcom);

        Conta conta = new Conta(
            "Cliente Teste",
            mesa.Id,
            mesa.Identificacao,
            garcom.Id,
            garcom.Nome
        );

        conta.Fechar(garcom.Nome, mesa.Identificacao);

        repositorioConta.Cadastrar(conta);
        dbContext.ChangeTracker.Clear();

        bool existeContaAberta =
            repositorioConta.ExisteContaAbertaPorGarcom(garcom.Id);

        Assert.IsFalse(existeContaAberta);
    }

    [TestMethod]
    public void Excluir_ContaCadastrada_RemoveConta()
    {
        Mesa mesa = new Mesa("Mesa 01", 4);
        Garcom garcom = new Garcom(
            "João",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        repositorioMesa.Cadastrar(mesa);
        repositorioGarcom.Cadastrar(garcom);

        Conta conta = new Conta(
            "Cliente Teste",
            mesa.Id,
            mesa.Identificacao,
            garcom.Id,
            garcom.Nome
        );

        repositorioConta.Cadastrar(conta);
        dbContext.ChangeTracker.Clear();

        bool conseguiuExcluir =
            repositorioConta.Excluir(conta.Id);

        dbContext.ChangeTracker.Clear();

        Conta? contaSelecionada =
            repositorioConta.SelecionarPorId(conta.Id);

        Assert.IsTrue(conseguiuExcluir);
        Assert.IsNull(contaSelecionada);
    }
}