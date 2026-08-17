using ControleDeBar.Dominio.Modulos.ModuloMesa;

namespace ControleDeBar.Aplicacao.Modulos.ModuloMesa;

public record ListarMesasDto(
    Guid Id,
    string Identificacao,
    int QuantidadeLugar,
    StatusMesa StatusMesa
);

public record CadastrarMesaDto(
    string Identificacao,
    int QuantidadeLugar,
    StatusMesa StatusMesa
);

public record EditarMesaDto(
    Guid Id,
    string Identificacao,
    int QuantidadeLugar,
    StatusMesa StatusMesa
);

public record DetalhesMesaDto(
    Guid Id,
    string Identificacao,
    int QuantidadeLugar,
    StatusMesa StatusMesa
);