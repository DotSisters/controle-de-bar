using ControleDeBar.Dominio.Modulos.ModuloMesa;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloMesa;

[TestClass]
public sealed class MesaTests
{
    [TestMethod]
    public void Validar_DeveCadastrar_ComDadosValidos()
    {
        Mesa mesa = new("Mesa01", 2);

        List<string> erros = mesa.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_ComIdentificacaoVazia_DeveRetornarErro()
    {
        Mesa mesa = new(string.Empty, 3);

        List<string> erros = mesa.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Identificação\" deve ser preenchido.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_ComQuantidadeLugarVazio_DeveRetornarErro()
    {
        Mesa mesa = new("Mesa05", 0);

        List<string> erros = mesa.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Quantidade de Lugares\" deve ser maior que zero.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_ComIdentificacaoCurta_DeveRetornarErro()
    {
        Mesa mesa = new(new string('A', 1), 7);

        List<string> erros = mesa.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Identificação\" deve conter no mínimo 2 caracteres.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_ComIdentificacaoTamanhoLimite()
    {
        Mesa mesa = new(new string('A', 3), 7);

        List<string> erros = mesa.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_ComIdentificacaoLonga_DeveRetornarErro()
    {
        Mesa mesa = new(new string('A', 101), 7);

        List<string> erros = mesa.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Identificação\" deve conter no máximo 20 caracteres.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_ComIdentificacaoTamanhoMaximo()
    {
        Mesa mesa = new(new string('A', 100), 7);

        List<string> erros = mesa.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_ComQuantidadeLugarZero_DeveRetornarErro()
    {
        Mesa mesa = new(new string('A', 4), 0);

        List<string> erros = mesa.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Quantidade de Lugares\" deve ser maior que zero.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_ComQuantidadeLugarNegativo_DeveRetornarErro()
    {
        Mesa mesa = new(new string('A', 4), -3);

        List<string> erros = mesa.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Quantidade de Lugares\" deve ser maior que zero.",
            erros.First()
        );
    }

    [TestMethod]
    public void Mesa_DeveIniciarComoLivre()
    {
        Mesa mesa = new("A123", 4);

        Assert.AreEqual(StatusMesa.Livre, mesa.StatusMesa);
    }

    [TestMethod]
    public void Mesa_MarcarComoOcupada_DeveAlterarStatus()
    {
        Mesa mesa = new("Mesa01", 4);
        mesa.MarcarComoOcupada();
        Assert.AreEqual(StatusMesa.Ocupada, mesa.StatusMesa);
    }

    [TestMethod]
    public void Mesa_MarcarComoLivre_DeveAlterarStatus()
    {
        Mesa mesa = new("Mesa01", 4);
        mesa.MarcarComoOcupada();
        mesa.MarcarComoLivre();
        Assert.AreEqual(StatusMesa.Livre, mesa.StatusMesa);
    }

    [TestMethod]
    public void Mesa_Atualizar_DeveAlterarCampos()
    {
        Mesa mesa = new("Mesa01", 4);
        Mesa novaMesa = new("Mesa02", 6);
        novaMesa.MarcarComoOcupada();

        mesa.Atualizar(novaMesa);

        Assert.AreEqual("Mesa02", mesa.Identificacao);
        Assert.AreEqual(6, mesa.QuantidadeLugar);
        Assert.AreEqual(StatusMesa.Ocupada, mesa.StatusMesa);
    }

    [TestMethod]
    public void Validar_ComStatusInvalido_DeveRetornarErro()
    {
        Mesa mesa = new("Mesa01", 4);
        mesa.MarcarComoOcupada();

        List<string> erros = mesa.Validar();

        Assert.IsTrue(erros.Contains("Uma mesa cadastrada deve iniciar com o status Livre."));
    }
}