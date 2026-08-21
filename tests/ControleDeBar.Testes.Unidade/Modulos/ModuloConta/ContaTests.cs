using ControleDeBar.Dominio.Modulos.ModuloConta;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloConta;

[TestClass]
public sealed class ContaTests
{
    [TestMethod]
    public void Validar_TodosDadosValidos_NaoRetornaErros()
    {
        Conta conta = new Conta(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        List<string> erros = conta.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Construtor_DeveAbrirContaEGravarDados()
    {
        Guid mesaId = Guid.CreateVersion7();
        Guid garcomId = Guid.CreateVersion7();

        Conta conta = new Conta(
            "João Silva",
            mesaId,
            "Mesa 01",
            garcomId,
            "Ana"
        );

        Assert.AreEqual("João Silva", conta.NomeCliente);
        Assert.AreEqual("Mesa 01", conta.IdentificacaoMesa);
        Assert.AreEqual(mesaId, conta.MesaId);
        Assert.AreEqual("Ana", conta.NomeGarcom);
        Assert.AreEqual(garcomId, conta.GarcomId);
        Assert.AreEqual(SituacaoConta.Aberta, conta.Situacao);
        Assert.IsTrue(conta.EstaAberta);
    }

    [TestMethod]
    public void Cadastrar_SemCamposObrigatorios_RetornaErros()
    {
        Conta conta = new Conta(
            string.Empty,
            Guid.Empty,
            string.Empty,
            Guid.Empty,
            string.Empty
        );

        List<string> erros = conta.Validar();

        Assert.IsTrue(
            erros.Contains("O campo \"Nome do Cliente\" deve ser preenchido.")
        );

        Assert.IsTrue(
            erros.Contains("O campo \"Mesa\" deve ser preenchido.")
        );

        Assert.IsTrue(
            erros.Contains("O campo \"Garçom\" deve ser preenchido.")
        );
    }

    [TestMethod]
    public void Validar_NomeClienteAbaixoDoMinimo_RetornaErro()
    {
        Conta conta = new Conta(
            new string('A', 2),
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        List<string> erros = conta.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Nome do Cliente\" deve conter no mínimo 3 caracteres.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_NomeClienteNoTamanhoMinimo_NaoRetornaErro()
    {
        Conta conta = new Conta(
            new string('A', 3),
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        List<string> erros = conta.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_NomeClienteAcimaDoMaximo_RetornaErro()
    {
        Conta conta = new Conta(
            new string('A', 101),
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        List<string> erros = conta.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Nome do Cliente\" deve conter no máximo 100 caracteres.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_NomeClienteNoTamanhoMaximo_NaoRetornaErro()
    {
        Conta conta = new Conta(
            new string('A', 100),
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        List<string> erros = conta.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_SemMesaId_RetornaErro()
    {
        Conta conta = new Conta(
            "João Silva",
            Guid.Empty,
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        List<string> erros = conta.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Mesa\" deve ser preenchido.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_SemIdentificacaoMesa_RetornaErro()
    {
        Conta conta = new Conta(
            "João Silva",
            Guid.CreateVersion7(),
            string.Empty,
            Guid.CreateVersion7(),
            "Ana"
        );

        List<string> erros = conta.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Mesa\" deve ser preenchido.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_SemGarcomId_RetornaErro()
    {
        Conta conta = new Conta(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.Empty,
            "Ana"
        );

        List<string> erros = conta.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Garçom\" deve ser preenchido.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_SemNomeGarcom_RetornaErro()
    {
        Conta conta = new Conta(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            string.Empty
        );

        List<string> erros = conta.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Garçom\" deve ser preenchido.",
            erros.First()
        );
    }

    [TestMethod]
    public void Construtor_RegistraDataDeAbertura()
    {
        Conta conta = new Conta(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        Assert.AreNotEqual(default(DateTime), conta.DataAbertura);
        Assert.AreEqual(DateTime.Today, conta.DataAbertura.Date);
    }

    [TestMethod]
    public void Atualizar_DeveTrocarDadosDaConta()
    {
        Conta conta = new Conta(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        Conta contaAtualizada = new Conta(
            "Maria Souza",
            Guid.CreateVersion7(),
            "Mesa 02",
            Guid.CreateVersion7(),
            "Carlos"
        );

        conta.Atualizar(contaAtualizada);

        Assert.AreEqual("Maria Souza", conta.NomeCliente);
        Assert.AreEqual("Mesa 02", conta.IdentificacaoMesa);
        Assert.AreEqual(contaAtualizada.MesaId, conta.MesaId);
        Assert.AreEqual("Carlos", conta.NomeGarcom);
        Assert.AreEqual(contaAtualizada.GarcomId, conta.GarcomId);
    }

    [TestMethod]
    public void Fechar_DeveAtualizarSnapshotsEMarcarComoFechada()
    {
        Conta conta = new Conta(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        conta.Fechar("Carlos", "Mesa 05");

        Assert.AreEqual("Carlos", conta.NomeGarcom);
        Assert.AreEqual("Mesa 05", conta.IdentificacaoMesa);
        Assert.AreEqual(SituacaoConta.Fechada, conta.Situacao);
        Assert.IsTrue(conta.EstaFechada);
        Assert.IsFalse(conta.EstaAberta);
    }

    [TestMethod]
    public void AdicionarItem_DeveCalcularTotalDaConta()
    {
        Conta conta = new Conta(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        ItemPedido item = new ItemPedido(
            conta.Id,
            Guid.CreateVersion7(),
            2,
            10m
        );

        conta.AdicionarItem(item);

        Assert.HasCount(1, conta.Itens);
        Assert.AreEqual(20m, conta.ValorTotal);
    }

    [TestMethod]
    public void RecalcularValorTotal_ComVariosItens_DeveSomarValores()
    {
        Conta conta = new Conta(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        ItemPedido primeiroItem = new ItemPedido(
            conta.Id,
            Guid.CreateVersion7(),
            2,
            10m
        );

        ItemPedido segundoItem = new ItemPedido(
            conta.Id,
            Guid.CreateVersion7(),
            3,
            10m
        );

        conta.AdicionarItem(primeiroItem);
        conta.AdicionarItem(segundoItem);

        Assert.AreEqual(50m, conta.ValorTotal);
    }

    [TestMethod]
    public void RecalcularValorTotal_AposRemoverItem_DeveAtualizarTotal()
    {
        Conta conta = new Conta(
            "João Silva",
            Guid.CreateVersion7(),
            "Mesa 01",
            Guid.CreateVersion7(),
            "Ana"
        );

        ItemPedido primeiroItem = new ItemPedido(
            conta.Id,
            Guid.CreateVersion7(),
            2,
            10m
        );

        ItemPedido segundoItem = new ItemPedido(
            conta.Id,
            Guid.CreateVersion7(),
            3,
            10m
        );

        conta.AdicionarItem(primeiroItem);
        conta.AdicionarItem(segundoItem);

        conta.RecalcularValorTotal(
            conta.Itens
                .Where(item => item.Id != segundoItem.Id)
                .Select(item => item.Valor)
        );

        Assert.AreEqual(20m, conta.ValorTotal);
    }
}