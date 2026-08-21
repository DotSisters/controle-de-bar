using ControleDeBar.Aplicacao.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using FluentResults;
using Moq;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloItemPedido;

[TestClass]
public sealed class ServicoItemPedidoTests
{
    [TestMethod]
    public void AdicionarPedido_ContaFechada_RetornaErro()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Conta conta = new(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        Produto produto = new("Cerveja", 20.00m);
        ItemPedido itemExistente = new(conta.Id, produto.Id, 1, produto.Valor);
        conta.AdicionarItem(itemExistente);
        conta.Fechar("Ana", "Mesa 01");

        int quantidadeItensAntes = conta.Itens.Count;
        decimal totalAntes = conta.ValorTotal;

        repositorioConta
            .Setup(r => r.SelecionarPorId(conta.Id))
            .Returns(conta);

        repositorioProduto
            .Setup(r => r.SelecionarPorId(produto.Id))
            .Returns(produto);

        ServicoConta servicoConta = new(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.AdicionarPedido(new AdicionarPedidoContaDto(
            conta.Id,
            produto.Id,
            2
        ));

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Não é possível adicionar itens de pedido a uma conta fechada.",
            resultado.Errors.First().Message
        );
        Assert.AreEqual(SituacaoConta.Fechada, conta.Situacao);
        Assert.HasCount(quantidadeItensAntes, conta.Itens);
        Assert.AreEqual(totalAntes, conta.ValorTotal);

        repositorioItemPedido.Verify(
            r => r.Cadastrar(It.IsAny<ItemPedido>()),
            Times.Never
        );
    }

    [TestMethod]
    public void AdicionarPedido_ComQuantidadeEPreco_CalculaValorDoItem()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Conta conta = new(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        Produto produto = new("Cerveja", 7.50m);

        ItemPedido? itemCadastrado = null;

        repositorioConta
            .Setup(r => r.SelecionarPorId(conta.Id))
            .Returns(conta);

        repositorioProduto
            .Setup(r => r.SelecionarPorId(produto.Id))
            .Returns(produto);

        repositorioItemPedido
            .Setup(r => r.Cadastrar(It.IsAny<ItemPedido>()))
            .Callback<ItemPedido>(item => itemCadastrado = item);

        ServicoConta servicoConta = new(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.AdicionarPedido(new AdicionarPedidoContaDto(
            conta.Id,
            produto.Id,
            3
        ));

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(itemCadastrado);
        Assert.AreEqual(produto.Id, itemCadastrado.ProdutoId);
        Assert.AreEqual(3, itemCadastrado.Quantidade);
        Assert.AreEqual(7.50m, itemCadastrado.ValorUnitario);
        Assert.AreEqual(22.50m, itemCadastrado.Valor);
        Assert.HasCount(1, conta.Itens);
        Assert.AreEqual(22.50m, conta.ValorTotal);

        repositorioItemPedido.Verify(
            r => r.Cadastrar(It.IsAny<ItemPedido>()),
            Times.Once
        );
    }

    [TestMethod]
    public void SelecionarPorId_ContaAbertaComItens_RetornaTodosItensPedido()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Conta conta = new(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        Produto cerveja = new("Cerveja", 7.50m);
        Produto suco = new("Suco de Laranja", 5.00m);

        ItemPedido itemCerveja = new(conta.Id, cerveja.Id, 3, cerveja.Valor);
        ItemPedido itemSuco = new(conta.Id, suco.Id, 2, suco.Valor);

        typeof(ItemPedido).GetProperty(nameof(ItemPedido.Produto))!
            .SetValue(itemCerveja, cerveja);
        typeof(ItemPedido).GetProperty(nameof(ItemPedido.Produto))!
            .SetValue(itemSuco, suco);

        conta.AdicionarItem(itemCerveja);
        conta.AdicionarItem(itemSuco);

        repositorioConta
            .Setup(r => r.SelecionarPorId(conta.Id))
            .Returns(conta);

        ServicoConta servicoConta = new(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result<DetalhesContaDto> resultado = servicoConta.SelecionarPorId(conta.Id);

        Assert.IsTrue(resultado.IsSuccess);
        Assert.IsNotNull(resultado.Value);
        Assert.AreEqual(SituacaoConta.Aberta, resultado.Value.Situacao);
        Assert.HasCount(2, resultado.Value.Pedidos);
        Assert.AreEqual(32.50m, resultado.Value.ValorTotal);

        ItemPedidoContaDto pedidoCerveja = resultado.Value.Pedidos.Single(p => p.Id == itemCerveja.Id);
        Assert.AreEqual("Cerveja", pedidoCerveja.NomeProduto);
        Assert.AreEqual(3, pedidoCerveja.Quantidade);
        Assert.AreEqual(7.50m, pedidoCerveja.ValorUnitario);
        Assert.AreEqual(22.50m, pedidoCerveja.ValorTotal);

        ItemPedidoContaDto pedidoSuco = resultado.Value.Pedidos.Single(p => p.Id == itemSuco.Id);
        Assert.AreEqual("Suco de Laranja", pedidoSuco.NomeProduto);
        Assert.AreEqual(2, pedidoSuco.Quantidade);
        Assert.AreEqual(5.00m, pedidoSuco.ValorUnitario);
        Assert.AreEqual(10.00m, pedidoSuco.ValorTotal);
    }

    [TestMethod]
    public void AlterarQuantidadeItemPedido_ContaAberta_AtualizaQuantidadeValorETotal()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Conta conta = new(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        Produto produto = new("Cerveja", 20.00m);
        ItemPedido itemPedido = new(conta.Id, produto.Id, 2, produto.Valor);
        conta.AdicionarItem(itemPedido);

        repositorioConta
            .Setup(r => r.SelecionarPorId(conta.Id))
            .Returns(conta);

        repositorioItemPedido
            .Setup(r => r.Editar(itemPedido.Id, itemPedido))
            .Returns(true);

        ServicoConta servicoConta = new(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        int novaQuantidade = 5;

        Result resultado = servicoConta.AlterarQuantidadeItemPedido(
            new AlterarQuantidadeItemPedidoDto(conta.Id, itemPedido.Id, novaQuantidade)
        );

        decimal valorEsperado = produto.Valor * novaQuantidade;

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(SituacaoConta.Aberta, conta.Situacao);
        Assert.AreEqual(novaQuantidade, itemPedido.Quantidade);
        Assert.AreEqual(valorEsperado, itemPedido.Valor);
        Assert.AreEqual(valorEsperado, conta.ValorTotal);

        repositorioItemPedido.Verify(
            r => r.Editar(itemPedido.Id, itemPedido),
            Times.Once
        );
    }

    [TestMethod]
    public void AlterarQuantidadeItemPedido_ContaFechada_RetornaErro()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Conta conta = new(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        Produto produto = new("Cerveja", 20.00m);
        ItemPedido itemPedido = new(conta.Id, produto.Id, 2, produto.Valor);
        conta.AdicionarItem(itemPedido);

        int quantidadeAntes = itemPedido.Quantidade;
        decimal valorAntes = itemPedido.Valor;
        decimal totalAntes = conta.ValorTotal;

        conta.Fechar("Ana", "Mesa 01");

        repositorioConta
            .Setup(r => r.SelecionarPorId(conta.Id))
            .Returns(conta);

        ServicoConta servicoConta = new(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.AlterarQuantidadeItemPedido(
            new AlterarQuantidadeItemPedidoDto(conta.Id, itemPedido.Id, 5)
        );

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Não é possível alterar a quantidade de um item de uma conta fechada.",
            resultado.Errors.First().Message
        );
        Assert.AreEqual(SituacaoConta.Fechada, conta.Situacao);
        Assert.AreEqual(quantidadeAntes, itemPedido.Quantidade);
        Assert.AreEqual(valorAntes, itemPedido.Valor);
        Assert.AreEqual(totalAntes, conta.ValorTotal);

        repositorioItemPedido.Verify(
            r => r.Editar(It.IsAny<Guid>(), It.IsAny<ItemPedido>()),
            Times.Never
        );
    }

    [TestMethod]
    public void RemoverItemPedido_ContaAberta_RemoveItemERecalculaTotal()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Conta conta = new(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        Produto produto = new("Cerveja", 20.00m);
        ItemPedido itemRemovido = new(conta.Id, produto.Id, 2, produto.Valor);
        ItemPedido itemRestante = new(conta.Id, produto.Id, 1, produto.Valor);
        conta.AdicionarItem(itemRemovido);
        conta.AdicionarItem(itemRestante);

        repositorioConta
            .Setup(r => r.SelecionarPorId(conta.Id))
            .Returns(conta);

        repositorioItemPedido
            .Setup(r => r.Excluir(itemRemovido.Id))
            .Returns(true);

        ServicoConta servicoConta = new(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.RemoverItemPedido(
            new RemoverItemPedidoDto(conta.Id, itemRemovido.Id)
        );

        Assert.IsTrue(resultado.IsSuccess);
        Assert.AreEqual(SituacaoConta.Aberta, conta.Situacao);
        Assert.HasCount(1, conta.Itens);
        Assert.IsFalse(conta.Itens.Contains(itemRemovido));
        Assert.AreEqual(itemRestante, conta.Itens.Single());
        Assert.AreEqual(itemRestante.Valor, conta.ValorTotal);

        repositorioItemPedido.Verify(
            r => r.Excluir(itemRemovido.Id),
            Times.Once
        );
    }

    [TestMethod]
    public void RemoverItemPedido_ContaFechada_RetornaErro()
    {
        Mock<IRepositorioConta> repositorioConta = new();
        Mock<IRepositorioMesa> repositorioMesa = new();
        Mock<IRepositorioGarcom> repositorioGarcom = new();
        Mock<IRepositorioProduto> repositorioProduto = new();
        Mock<IRepositorioItemPedido> repositorioItemPedido = new();

        Conta conta = new(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        Produto produto = new("Cerveja", 20.00m);
        ItemPedido itemPedido = new(conta.Id, produto.Id, 2, produto.Valor);
        conta.AdicionarItem(itemPedido);

        int quantidadeItensAntes = conta.Itens.Count;
        decimal totalAntes = conta.ValorTotal;

        conta.Fechar("Ana", "Mesa 01");

        repositorioConta
            .Setup(r => r.SelecionarPorId(conta.Id))
            .Returns(conta);

        ServicoConta servicoConta = new(
            repositorioConta.Object,
            repositorioMesa.Object,
            repositorioGarcom.Object,
            repositorioProduto.Object,
            repositorioItemPedido.Object
        );

        Result resultado = servicoConta.RemoverItemPedido(
            new RemoverItemPedidoDto(conta.Id, itemPedido.Id)
        );

        Assert.IsTrue(resultado.IsFailed);
        Assert.AreEqual(
            "Não é possível remover itens de uma conta fechada.",
            resultado.Errors.First().Message
        );
        Assert.AreEqual(SituacaoConta.Fechada, conta.Situacao);
        Assert.HasCount(quantidadeItensAntes, conta.Itens);
        Assert.IsTrue(conta.Itens.Contains(itemPedido));
        Assert.AreEqual(totalAntes, conta.ValorTotal);

        repositorioItemPedido.Verify(
            r => r.Excluir(It.IsAny<Guid>()),
            Times.Never
        );
    }
}
