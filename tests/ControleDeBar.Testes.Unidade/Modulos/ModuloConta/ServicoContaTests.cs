using ControleDeBar.Aplicacao.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using FluentResults;
using Moq;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloConta;

[TestClass]
public sealed class ServicoContaTests
{
    [TestMethod]
    public void Cadastrar_ComTodosCampos_PersisteContaEMarcaMesaComoOcupada()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Mesa mesa = new Mesa("Mesa 01", 4);
        Garcom garcom = new Garcom(
            "João",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        repositorioMesa
            .Setup(r => r.SelecionarPorId(mesa.Id))
            .Returns(mesa);

        repositorioConta
            .Setup(r => r.ExisteContaAbertaPorMesa(mesa.Id))
            .Returns(false);

        repositorioGarcom
            .Setup(r => r.SelecionarPorId(garcom.Id))
            .Returns(garcom);

        Conta? contaCadastrada = null;

        repositorioConta
            .Setup(r => r.Cadastrar(It.IsAny<Conta>()))
            .Callback<Conta>(
                conta => contaCadastrada = conta
            );

        ServicoConta servicoConta = new ServicoConta(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.Cadastrar(
            new CadastrarContaDto(
                "Cliente Teste",
                mesa.Id,
                garcom.Id
            )
        );

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(contaCadastrada);
        Assert.AreEqual("Cliente Teste", contaCadastrada.NomeCliente);
        Assert.AreEqual(mesa.Id, contaCadastrada.MesaId);
        Assert.AreEqual(garcom.Id, contaCadastrada.GarcomId);
        Assert.AreEqual(StatusMesa.Ocupada, mesa.StatusMesa);

        repositorioConta.Verify(
            r => r.Cadastrar(It.IsAny<Conta>()),
            Times.Once
        );
    }

    [TestMethod]
    public void Cadastrar_SemNomeCliente_RetornaErro()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Mesa mesa = new Mesa("Mesa 01", 4);
        Garcom garcom = new Garcom(
            "João",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        repositorioMesa
            .Setup(r => r.SelecionarPorId(mesa.Id))
            .Returns(mesa);

        repositorioConta
            .Setup(r => r.ExisteContaAbertaPorMesa(mesa.Id))
            .Returns(false);

        repositorioGarcom
            .Setup(r => r.SelecionarPorId(garcom.Id))
            .Returns(garcom);

        ServicoConta servicoConta = new ServicoConta(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.Cadastrar(
            new CadastrarContaDto(
                string.Empty,
                mesa.Id,
                garcom.Id
            )
        );

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "O campo \"Nome do Cliente\" deve ser preenchido.",
            resultado.Errors.First().Message
        );

        repositorioConta.Verify(
            r => r.Cadastrar(It.IsAny<Conta>()),
            Times.Never
        );
    }

    [TestMethod]
    public void Cadastrar_SemMesa_RetornaErro()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        repositorioMesa
            .Setup(r => r.SelecionarPorId(It.IsAny<Guid>()))
            .Returns((Mesa?)null);

        ServicoConta servicoConta = new ServicoConta(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.Cadastrar(
            new CadastrarContaDto(
                "Cliente Teste",
                Guid.NewGuid(),
                Guid.NewGuid()
            )
        );

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Mesa não encontrada.",
            resultado.Errors.First().Message
        );
    }

    [TestMethod]
    public void Cadastrar_SemGarcom_RetornaErro()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Mesa mesa = new Mesa("Mesa 01", 4);

        repositorioMesa
            .Setup(r => r.SelecionarPorId(mesa.Id))
            .Returns(mesa);

        repositorioGarcom
            .Setup(r => r.SelecionarPorId(It.IsAny<Guid>()))
            .Returns((Garcom?)null);

        ServicoConta servicoConta = new ServicoConta(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.Cadastrar(
            new CadastrarContaDto(
                "Cliente Teste",
                mesa.Id,
                Guid.NewGuid()
            )
        );

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Garçom não encontrado.",
            resultado.Errors.First().Message
        );
    }

    [TestMethod]
    public void Cadastrar_ComMesaOcupada_RetornaErro()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Mesa mesa = new Mesa("Mesa 01", 4);

        repositorioMesa
            .Setup(r => r.SelecionarPorId(mesa.Id))
            .Returns(mesa);

        repositorioConta
            .Setup(r => r.ExisteContaAbertaPorMesa(mesa.Id))
            .Returns(true);

        ServicoConta servicoConta = new ServicoConta(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.Cadastrar(
            new CadastrarContaDto(
                "Cliente Teste",
                mesa.Id,
                Guid.NewGuid()
            )
        );

        Assert.IsTrue(resultado.IsFailed);
        Assert.Contains(
            "já possui uma conta em aberto",
            resultado.Errors.First().Message
        );

        repositorioConta.Verify(
            r => r.Cadastrar(It.IsAny<Conta>()),
            Times.Never
        );
    }

    [TestMethod]
    public void Cadastrar_RegistraDataDeAbertura()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Mesa mesa = new Mesa("Mesa 01", 4);
        Garcom garcom = new Garcom(
            "João",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        Conta? contaCadastrada = null;

        repositorioMesa
            .Setup(r => r.SelecionarPorId(mesa.Id))
            .Returns(mesa);

        repositorioConta
            .Setup(r => r.ExisteContaAbertaPorMesa(mesa.Id))
            .Returns(false);

        repositorioGarcom
            .Setup(r => r.SelecionarPorId(garcom.Id))
            .Returns(garcom);

        repositorioConta
            .Setup(r => r.Cadastrar(It.IsAny<Conta>()))
            .Callback<Conta>(
                conta => contaCadastrada = conta
            );

        ServicoConta servicoConta = new ServicoConta(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.Cadastrar(
            new CadastrarContaDto(
                "Cliente Teste",
                mesa.Id,
                garcom.Id
            )
        );

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(contaCadastrada);
        Assert.AreEqual(
            DateTime.Today,
            contaCadastrada.DataAbertura.Date
        );
    }

    [TestMethod]
    public void Fechar_SemPedidos_LiberaMesa()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Mesa mesa = new Mesa("Mesa 01", 4);
        Garcom garcom = new Garcom(
            "João",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        Conta conta = new Conta(
            "Cliente Teste",
            mesa.Id,
            mesa.Identificacao,
            garcom.Id,
            garcom.Nome
        );

        repositorioConta
            .Setup(r => r.SelecionarPorId(conta.Id))
            .Returns(conta);

        repositorioMesa
            .Setup(r => r.SelecionarPorId(mesa.Id))
            .Returns(mesa);

        repositorioGarcom
            .Setup(r => r.SelecionarPorId(garcom.Id))
            .Returns(garcom);

        repositorioConta
            .Setup(r => r.Editar(conta.Id, conta))
            .Returns(true);

        ServicoConta servicoConta = new ServicoConta(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.Fechar(conta.Id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsTrue(conta.EstaFechada);
        Assert.AreEqual(StatusMesa.Livre, mesa.StatusMesa);

        repositorioConta.Verify(
            r => r.Editar(conta.Id, conta),
            Times.Once
        );
    }

    [TestMethod]
    public void Fechar_ComPedidosVinculados_LiberaMesa()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Mesa mesa = new Mesa("Mesa 01", 4);
        Garcom garcom = new Garcom(
            "João",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        Conta conta = new Conta(
            "Cliente Teste",
            mesa.Id,
            mesa.Identificacao,
            garcom.Id,
            garcom.Nome
        );

        ItemPedido item = new ItemPedido(
            conta.Id,
            Guid.NewGuid(),
            2,
            10m
        );

        conta.AdicionarItem(item);

        repositorioConta
            .Setup(r => r.SelecionarPorId(conta.Id))
            .Returns(conta);

        repositorioMesa
            .Setup(r => r.SelecionarPorId(mesa.Id))
            .Returns(mesa);

        repositorioGarcom
            .Setup(r => r.SelecionarPorId(garcom.Id))
            .Returns(garcom);

        repositorioConta
            .Setup(r => r.Editar(conta.Id, conta))
            .Returns(true);

        ServicoConta servicoConta = new ServicoConta(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.Fechar(conta.Id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsTrue(conta.EstaFechada);
        Assert.AreEqual(StatusMesa.Livre, mesa.StatusMesa);
    }

    [TestMethod]
    public void Editar_ContaAberta_ComDadosValidos_PersisteAlteracoes()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Mesa mesa = new Mesa("Mesa 01", 4);
        Garcom garcom = new Garcom(
            "João",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        Conta conta = new Conta(
            "Cliente Teste",
            mesa.Id,
            mesa.Identificacao,
            garcom.Id,
            garcom.Nome
        );

        repositorioConta
            .Setup(r => r.SelecionarPorId(conta.Id))
            .Returns(conta);

        repositorioMesa
            .Setup(r => r.SelecionarPorId(mesa.Id))
            .Returns(mesa);

        repositorioGarcom
            .Setup(r => r.SelecionarPorId(garcom.Id))
            .Returns(garcom);

        repositorioConta
            .Setup(r => r.Editar(
                conta.Id,
                It.IsAny<Conta>()))
            .Returns(true);

        ServicoConta servicoConta = new ServicoConta(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.Editar(
            new EditarContaDto(
                conta.Id,
                "Cliente Alterado",
                mesa.Id,
                garcom.Id
            )
        );

        Assert.IsTrue(resultado.IsSuccess);

        repositorioConta.Verify(
            r => r.Editar(
                conta.Id,
                It.IsAny<Conta>()),
            Times.Once
        );
    }

    [TestMethod]
    public void Editar_ContaFechada_RetornaErro()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Mesa mesa = new Mesa("Mesa 01", 4);
        Garcom garcom = new Garcom(
            "João",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        Conta conta = new Conta(
            "Cliente Teste",
            mesa.Id,
            mesa.Identificacao,
            garcom.Id,
            garcom.Nome
        );

        conta.Fechar(
            garcom.Nome,
            mesa.Identificacao
        );

        repositorioConta
            .Setup(r => r.SelecionarPorId(conta.Id))
            .Returns(conta);

        ServicoConta servicoConta = new ServicoConta(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.Editar(
            new EditarContaDto(
                conta.Id,
                "Cliente Alterado",
                mesa.Id,
                garcom.Id
            )
        );

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Não é possível editar uma conta fechada.",
            resultado.Errors.First().Message
        );

        repositorioConta.Verify(
            r => r.Editar(
                It.IsAny<Guid>(),
                It.IsAny<Conta>()),
            Times.Never
        );
    }

    [TestMethod]
    public void SelecionarTodos_RetornaTodasAsContas()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Conta conta = new Conta(
            "Cliente Teste",
            Guid.NewGuid(),
            "Mesa 01",
            Guid.NewGuid(),
            "João"
        );

        repositorioConta
            .Setup(r => r.SelecionarTodos())
            .Returns(new List<Conta> { conta });

        ServicoConta servicoConta = new ServicoConta(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        List<ListarContasDto> resultado =
            servicoConta.SelecionarTodos();

        Assert.HasCount(1, resultado);
        Assert.AreEqual(conta.Id, resultado[0].Id);
        Assert.AreEqual(
            "Cliente Teste",
            resultado[0].NomeCliente
        );
    }

    [TestMethod]
    public void SelecionarPorId_ContaAberta_RetornaDetalhes()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Conta conta = new Conta(
            "Cliente Teste",
            Guid.NewGuid(),
            "Mesa 01",
            Guid.NewGuid(),
            "João"
        );

        repositorioConta
            .Setup(r => r.SelecionarPorId(conta.Id))
            .Returns(conta);

        ServicoConta servicoConta = new ServicoConta(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result<DetalhesContaDto> resultado =
            servicoConta.SelecionarPorId(conta.Id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(conta.Id, resultado.Value.Id);
        Assert.AreEqual(
            conta.NomeCliente,
            resultado.Value.NomeCliente
        );
        Assert.AreEqual(
            SituacaoConta.Aberta,
            resultado.Value.Situacao
        );
    }

    [TestMethod]
    public void SelecionarPorId_RetornaPedidosVinculados()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Conta conta = new Conta(
            "Cliente Teste",
            Guid.NewGuid(),
            "Mesa 01",
            Guid.NewGuid(),
            "João"
        );

        ItemPedido item = new ItemPedido(
            conta.Id,
            Guid.NewGuid(),
            2,
            10m
        );

        conta.AdicionarItem(item);

        repositorioConta
            .Setup(r => r.SelecionarPorId(conta.Id))
            .Returns(conta);

        ServicoConta servicoConta = new ServicoConta(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result<DetalhesContaDto> resultado =
            servicoConta.SelecionarPorId(conta.Id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.HasCount(1, resultado.Value.Pedidos);
        Assert.AreEqual(
            2,
            resultado.Value.Pedidos[0].Quantidade
        );
        Assert.AreEqual(
            20m,
            resultado.Value.Pedidos[0].ValorTotal
        );
    }

    [TestMethod]
    public void AdicionarPedido_ComUmPedido_CalculaTotal()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Conta conta = new Conta(
            "Cliente Teste",
            Guid.NewGuid(),
            "Mesa 01",
            Guid.NewGuid(),
            "João"
        );

        Produto produto = new Produto(
            "Produto Teste",
            10m
        );

        repositorioConta
            .Setup(r => r.SelecionarPorId(conta.Id))
            .Returns(conta);

        repositorioProduto
            .Setup(r => r.SelecionarPorId(produto.Id))
            .Returns(produto);

        ServicoConta servicoConta = new ServicoConta(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.AdicionarPedido(
            new AdicionarPedidoContaDto(
                conta.Id,
                produto.Id,
                2
            )
        );

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(20m, conta.ValorTotal);

        repositorioItemPedido.Verify(
            r => r.Cadastrar(It.IsAny<ItemPedido>()),
            Times.Once
        );
    }

    [TestMethod]
    public void AdicionarPedido_ComVariosPedidos_CalculaTotal()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Conta conta = new Conta(
            "Cliente Teste",
            Guid.NewGuid(),
            "Mesa 01",
            Guid.NewGuid(),
            "João"
        );

        Produto produto = new Produto(
            "Produto Teste",
            10m
        );

        repositorioConta
            .Setup(r => r.SelecionarPorId(conta.Id))
            .Returns(conta);

        repositorioProduto
            .Setup(r => r.SelecionarPorId(produto.Id))
            .Returns(produto);

        ServicoConta servicoConta = new ServicoConta(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        servicoConta.AdicionarPedido(
            new AdicionarPedidoContaDto(
                conta.Id,
                produto.Id,
                2
            )
        );

        servicoConta.AdicionarPedido(
            new AdicionarPedidoContaDto(
                conta.Id,
                produto.Id,
                3
            )
        );

        Assert.AreEqual(50m, conta.ValorTotal);
        Assert.HasCount(2, conta.Itens);
    }

    [TestMethod]
    public void AlterarQuantidade_DeveAtualizarTotal()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Conta conta = new Conta(
            "Cliente Teste",
            Guid.NewGuid(),
            "Mesa 01",
            Guid.NewGuid(),
            "João"
        );

        ItemPedido item = new ItemPedido(
            conta.Id,
            Guid.NewGuid(),
            2,
            10m
        );

        conta.AdicionarItem(item);

        repositorioConta
            .Setup(r => r.SelecionarPorId(conta.Id))
            .Returns(conta);

        repositorioItemPedido
            .Setup(r => r.Editar(item.Id, item))
            .Returns(true);

        ServicoConta servicoConta = new ServicoConta(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.AlterarQuantidadeItemPedido(
            new AlterarQuantidadeItemPedidoDto(
                conta.Id,
                item.Id,
                5
            )
        );

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(50m, conta.ValorTotal);

        repositorioItemPedido.Verify(
            r => r.Editar(item.Id, item),
            Times.Once
        );
    }

    [TestMethod]
    public void RemoverPedido_DeveAtualizarTotal()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Conta conta = new Conta(
            "Cliente Teste",
            Guid.NewGuid(),
            "Mesa 01",
            Guid.NewGuid(),
            "João"
        );

        ItemPedido item = new ItemPedido(
            conta.Id,
            Guid.NewGuid(),
            2,
            10m
        );

        conta.AdicionarItem(item);

        repositorioConta
            .Setup(r => r.SelecionarPorId(conta.Id))
            .Returns(conta);

        repositorioItemPedido
            .Setup(r => r.Excluir(item.Id))
            .Returns(true);

        ServicoConta servicoConta = new ServicoConta(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.RemoverItemPedido(
            new RemoverItemPedidoDto(
                conta.Id,
                item.Id
            )
        );

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(0m, conta.ValorTotal);

        repositorioItemPedido.Verify(
            r => r.Excluir(item.Id),
            Times.Once
        );
    }
}