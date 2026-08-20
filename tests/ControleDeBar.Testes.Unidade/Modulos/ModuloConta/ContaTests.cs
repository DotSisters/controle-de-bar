using ControleDeBar.Dominio.Modulos.ModuloConta;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloConta;

[TestClass]
public sealed class ContaTests
{
    [TestMethod]
    public void Construtor_DeveGravarIdentificacaoMesa()
    {
        Guid mesaId = Guid.CreateVersion7();
        Guid garcomId = Guid.CreateVersion7();

        Conta conta = new Conta("João Silva", mesaId, "Mesa 01", garcomId, "Ana");

        Assert.AreEqual("Mesa 01", conta.IdentificacaoMesa);
        Assert.AreEqual(mesaId, conta.MesaId);
        Assert.AreEqual("Ana", conta.NomeGarcom);
        Assert.AreEqual(garcomId, conta.GarcomId);
        Assert.AreEqual(SituacaoConta.Aberta, conta.Situacao);
    }

    [TestMethod]
    public void Atualizar_DeveTrocarSnapshotIdentificacaoMesa()
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
    }

    [TestMethod]
    public void Validar_SemMesaId_DeveRetornarErro()
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
        Assert.AreEqual("O campo \"Mesa\" deve ser preenchido.", erros.First());
    }
}
