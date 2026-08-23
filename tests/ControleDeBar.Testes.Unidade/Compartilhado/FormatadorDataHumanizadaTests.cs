using System.Globalization;
using ControleDeBar.Dominio.Compartilhado;

namespace ControleDeBar.Testes.Unidade.Compartilhado;

[TestClass]
public sealed class FormatadorDataHumanizadaTests
{
    private static readonly DateTime DataAtual = new(2026, 6, 18, 0, 0, 0);
    private static readonly CultureInfo Cultura = CultureInfo.GetCultureInfo("pt-BR");

    [TestMethod]
    [DataRow("18/06/2026 00:00:00", "agora mesmo")]
    [DataRow("17/06/2026 23:59:30", "há 30 segundos")]
    [DataRow("17/06/2026 23:55:00", "há 5 minutos")]
    [DataRow("17/06/2026 20:00:00", "há 4 horas")]
    [DataRow("17/06/2026 00:00:00", "um dia atrás")]
    [DataRow("16/06/2026 00:00:00", "dois dias atrás")]
    [DataRow("11/06/2026 00:00:00", "uma semana atrás")]
    [DataRow("04/06/2026 00:00:00", "duas semanas atrás")]
    [DataRow("11/05/2026 00:00:00", "um mês e uma semana atrás")]
    [DataRow("16/05/2026 00:00:00", "um mês e dois dias atrás")]
    [DataRow("18/06/2025 00:00:00", "um ano atrás")]
    [DataRow("18/06/2016 00:00:00", "dez anos atrás")]
    public void Formatar_ExemplosDaEspecificacao_RetornaTextoEsperado(
        string dataInformada,
        string esperado
    )
    {
        string resultado = FormatadorDataHumanizada.Formatar(
            Parse(dataInformada),
            DataAtual
        );

        Assert.AreEqual(esperado, resultado);
    }

    [TestMethod]
    [DataRow("17/06/2026 23:59:59", "há 1 segundo")]
    [DataRow("17/06/2026 23:59:00", "há 1 minuto")]
    [DataRow("17/06/2026 23:00:00", "há 1 hora")]
    public void Formatar_MenosDeUmDia_UsaSingularQuandoValorForUm(
        string dataInformada,
        string esperado
    )
    {
        string resultado = FormatadorDataHumanizada.Formatar(
            Parse(dataInformada),
            DataAtual
        );

        Assert.AreEqual(esperado, resultado);
    }

    [TestMethod]
    [DataRow("17/06/2026 19:01:00", "há 4 horas")]
    [DataRow("12/05/2026 00:00:00", "um mês e seis dias atrás")]
    public void Formatar_PeriodosIncompletos_NaoArredonda(
        string dataInformada,
        string esperado
    )
    {
        string resultado = FormatadorDataHumanizada.Formatar(
            Parse(dataInformada),
            DataAtual
        );

        Assert.AreEqual(esperado, resultado);
    }

    [TestMethod]
    [DataRow("01/01/2024 00:00:00", "dois anos e cinco meses atrás")]
    [DataRow("10/06/2026 00:00:00", "uma semana e um dia atrás")]
    [DataRow("17/06/2025 00:00:00", "um ano e um dia atrás")]
    public void Formatar_MaisDeUmDia_UsaNoMaximoDuasUnidades(
        string dataInformada,
        string esperado
    )
    {
        string resultado = FormatadorDataHumanizada.Formatar(
            Parse(dataInformada),
            DataAtual
        );

        Assert.AreEqual(esperado, resultado);
    }

    [TestMethod]
    public void Formatar_AnoBissexto_ContaAnoCompletoComAddYears()
    {
        DateTime dataInformada = new(2024, 2, 29, 0, 0, 0);
        DateTime dataAtual = new(2025, 2, 28, 0, 0, 0);

        string resultado = FormatadorDataHumanizada.Formatar(dataInformada, dataAtual);

        Assert.AreEqual("um ano atrás", resultado);
    }

    [TestMethod]
    public void Formatar_DataFutura_LancaArgumentOutOfRangeException()
    {
        DateTime dataFutura = new(2026, 6, 19, 0, 0, 0);

        ArgumentOutOfRangeException excecao = Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => FormatadorDataHumanizada.Formatar(dataFutura, DataAtual)
        );

        Assert.Contains(
            "A data informada não pode ser posterior à data atual.",
            excecao.Message
        );
    }

    private static DateTime Parse(string data)
    {
        return DateTime.ParseExact(data, "dd/MM/yyyy HH:mm:ss", Cultura);
    }
}
