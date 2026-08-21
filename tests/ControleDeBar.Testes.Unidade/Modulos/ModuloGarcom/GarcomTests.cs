using ControleDeBar.Dominio.Modulos.ModuloGarcom;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloGarcom;

[TestClass]
public sealed class GarcomTests
{
    [TestMethod]
    public void Validar_Todos_DadosValidos()
    {
        Garcom garcom = new Garcom("Teste", "(00) 00000-0000", "111.111.111-11");

        List<string> erros = garcom.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_NomeVazio_DeveRetornarErro()
    {
        Garcom garcom = new Garcom(string.Empty, "(00) 00000-0000", "111.111.111-11");

        List<string> erros = garcom.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Nome\" deve ser preenchido.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_TelefoneVazio_DeveRetornarErro()
    {
        Garcom garcom = new Garcom("Teste", string.Empty, "111.111.111-11");

        List<string> erros = garcom.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Telefone\" deve ser preenchido.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_CpfVazio_DeveRetornarErro()
    {
        Garcom garcom = new Garcom("Teste", "(00) 00000-0000", string.Empty);

        List<string> erros = garcom.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Cpf\" deve ser preenchido.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_NomeCurto_DeveRetornarErro()
    {
        Garcom garcom = new Garcom(new string('A', 1), "(00) 00000-0000", "111.111.111-11");

        List<string> erros = garcom.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Nome\" deve conter no mínimo 3 caracteres.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_Nome_ComTamanhoLimite()
    {
        Garcom garcom = new Garcom(new string('A', 3), "(00) 00000-0000", "111.111.111-11");

        List<string> erros = garcom.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_NomeLongo_DeveRetornarErro()
    {
        Garcom garcom = new Garcom(new string('A', 101), "(00) 00000-0000", "111.111.111-11");

        List<string> erros = garcom.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Nome\" deve conter no máximo 100 caracteres.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_Nome_ComTamanhoMaximo()
    {
        Garcom garcom = new Garcom(new string('A', 100), "(00) 00000-0000", "111.111.111-11");

        List<string> erros = garcom.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_Telefone_ComFormatoInvalido_DeveRetornarErro()
    {
        Garcom garcom = new Garcom("Teste", "00000000000", "111.111.111-11");

        List<string> erros = garcom.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Telefone\" deve estar no formato (XX) XXXX-XXXX ou (XX) XXXXX-XXXX.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_CpfCurto_DeveRetornarErro()
    {
        Garcom garcom = new Garcom("Teste", "(00) 00000-0000", "11.11.11-11");

        List<string> erros = garcom.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Cpf\" deve conter 11 dígitos.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_CpfLongo_DeveRetornarErro()
    {
        Garcom garcom = new Garcom("Teste", "(00) 00000-0000", "111.111.111-111");

        List<string> erros = garcom.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Cpf\" deve conter 11 dígitos.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_TelefoneFixo_Formatado()
    {
        Garcom garcom = new Garcom("Teste", "(00) 0000-0000", "111.111.111-11");

        List<string> erros = garcom.Validar();

        Assert.HasCount(0, erros);
    }

    [TestMethod]
    public void Validar_Cpf_ComFormatoInvalido_DeveRetornarErro()
    {
        Garcom garcom = new Garcom("Teste", "(00) 00000-0000", "11111111111");

        List<string> erros = garcom.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Cpf\" deve estar no formato XXX.XXX.XXX-XX.",
            erros.First()
        );
    }

    [TestMethod]
    public void Validar_Cpf_ComSeparadoresInvalidos()
    {
        Garcom garcom = new Garcom("Teste", "(00) 00000-0000", "111-111-111.11");

        List<string> erros = garcom.Validar();

        Assert.HasCount(1, erros);
        Assert.AreEqual(
            "O campo \"Cpf\" deve estar no formato XXX.XXX.XXX-XX.",
            erros.First()
        );
    }

    [TestMethod]
    public void Atualizar_DeveAtualizar_DadosValidos()
    {
        Garcom garcom = new Garcom(
            "Ana Silva",
            "(11) 11111-1111",
            "111.111.111-11"
        );

        Garcom garcomAtualizado = new Garcom(
            "Ana Maria Silva",
            "(00) 00000-0000",
            "000.000.000-00"
        );

        garcom.Atualizar(garcomAtualizado);

        Assert.AreEqual("Ana Maria Silva", garcom.Nome);
        Assert.AreEqual("(00) 00000-0000", garcom.Telefone);
        Assert.AreEqual("000.000.000-00", garcom.Cpf);
    }
}
