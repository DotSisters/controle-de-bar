

using System.ComponentModel.DataAnnotations;
using ControleDeBar.Dominio.Modulos.ModuloMesa;

namespace ControleDeBar.WebApp.Modulos.ModuloMesa;

public record ListarMesasViewModel(
    Guid Id,
    string Identificacao,
    int QuantidadeLugar,
    StatusMesa StatusMesa
);

public record CadastrarMesaViewModel(
    [Required(ErrorMessage = "O campo \"Identificação\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O campo \"Identificação\" deve conter entre 3 e 100 caracteres.")]
    string Identificacao,

    [Required(ErrorMessage = "O campo \"Quantidade de Lugares\" deve ser preenchido.")]
    [Range(1, 20, ErrorMessage = "O campo \"Quantidade de Lugares\" deve ser entre 1 e 20.")]
    int QuantidadeLugar,

    StatusMesa StatusMesa = StatusMesa.Livre
);

public record EditarMesaViewModel(
    Guid Id,

    [Required(ErrorMessage = "O campo \"Identificação\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O campo \"Identificação\" deve conter entre 3 e 100 caracteres.")]
    string Identificacao,

    [Required(ErrorMessage = "O campo \"Quantidade de Lugares\" deve ser preenchido.")]
    [Range(1, 20, ErrorMessage = "O campo \"Quantidade de Lugares\" deve ser entre 1 e 20.")]
    int QuantidadeLugar,

    StatusMesa StatusMesa = StatusMesa.Livre
);

public record ExcluirMesaViewModel(
    Guid Id,
    string Identificacao,
    int QuantidadeLugar,
    StatusMesa StatusMesa
);
