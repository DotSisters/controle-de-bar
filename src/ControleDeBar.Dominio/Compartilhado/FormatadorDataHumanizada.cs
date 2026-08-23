namespace ControleDeBar.Dominio.Compartilhado;

public static class FormatadorDataHumanizada
{
    private static readonly string[] Unidades =
    [
        "", "um", "dois", "três", "quatro", "cinco", "seis", "sete", "oito", "nove"
    ];

    private static readonly string[] DezADezenove =
    [
        "dez", "onze", "doze", "treze", "quatorze", "quinze",
        "dezesseis", "dezessete", "dezoito", "dezenove"
    ];

    private static readonly string[] Dezenas =
    [
        "", "", "vinte", "trinta", "quarenta", "cinquenta",
        "sessenta", "setenta", "oitenta", "noventa"
    ];

    public static string Formatar(DateTime dataInformada, DateTime dataAtual)
    {
        if (dataInformada > dataAtual)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dataInformada),
                dataInformada,
                "A data informada não pode ser posterior à data atual."
            );
        }

        TimeSpan decorrido = dataAtual - dataInformada;

        if (decorrido.TotalDays < 1)
            return FormatarMenosDeUmDia(decorrido);

        return FormatarUmDiaOuMais(dataInformada, dataAtual);
    }

    private static string FormatarMenosDeUmDia(TimeSpan decorrido)
    {
        if (decorrido.TotalSeconds < 1)
            return "agora mesmo";

        if (decorrido.TotalSeconds < 60)
        {
            int segundos = (int)decorrido.TotalSeconds;
            string unidade = segundos == 1 ? "segundo" : "segundos";
            return $"há {segundos} {unidade}";
        }

        if (decorrido.TotalMinutes < 60)
        {
            int minutos = (int)decorrido.TotalMinutes;
            string unidade = minutos == 1 ? "minuto" : "minutos";
            return $"há {minutos} {unidade}";
        }

        int horas = (int)decorrido.TotalHours;
        string unidadeHora = horas == 1 ? "hora" : "horas";
        return $"há {horas} {unidadeHora}";
    }

    private static string FormatarUmDiaOuMais(DateTime dataInformada, DateTime dataAtual)
    {
        DateTime cursor = dataInformada;
        int anos = 0;
        int meses = 0;

        while (cursor.AddYears(1) <= dataAtual)
        {
            cursor = cursor.AddYears(1);
            anos++;
        }

        while (cursor.AddMonths(1) <= dataAtual)
        {
            cursor = cursor.AddMonths(1);
            meses++;
        }

        int diasRestantes = (dataAtual - cursor).Days;
        int semanas = diasRestantes / 7;
        int dias = diasRestantes % 7;

        List<string> partes = [];

        AcrescentarUnidadePorExtenso(partes, anos, "ano", "anos", feminino: false);
        AcrescentarUnidadePorExtenso(partes, meses, "mês", "meses", feminino: false);
        AcrescentarUnidadePorExtenso(partes, semanas, "semana", "semanas", feminino: true);
        AcrescentarUnidadePorExtenso(partes, dias, "dia", "dias", feminino: false);

        IEnumerable<string> unidadesEscolhidas = partes.Take(2);
        string texto = string.Join(" e ", unidadesEscolhidas);

        return $"{texto} atrás";
    }

    private static void AcrescentarUnidadePorExtenso(
        List<string> partes,
        int valor,
        string singular,
        string plural,
        bool feminino
    )
    {
        if (valor <= 0)
            return;

        string numero = EscreverNumeroPorExtenso(valor, feminino);
        string unidade = valor == 1 ? singular : plural;
        partes.Add($"{numero} {unidade}");
    }

    private static string EscreverNumeroPorExtenso(int numero, bool feminino)
    {
        if (numero is < 1 or > 99)
            throw new ArgumentOutOfRangeException(nameof(numero));

        if (numero == 1)
            return feminino ? "uma" : "um";

        if (numero == 2)
            return feminino ? "duas" : "dois";

        if (numero < 10)
            return Unidades[numero];

        if (numero < 20)
            return DezADezenove[numero - 10];

        int dezena = numero / 10;
        int unidade = numero % 10;

        if (unidade == 0)
            return Dezenas[dezena];

        return $"{Dezenas[dezena]} e {EscreverNumeroPorExtenso(unidade, feminino)}";
    }
}
