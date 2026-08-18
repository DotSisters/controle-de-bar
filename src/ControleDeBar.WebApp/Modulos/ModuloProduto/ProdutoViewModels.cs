using System.ComponentModel.DataAnnotations;

namespace ControleDeBar.WebApp.Modulos.ModuloProduto;

public record ListarProdutosViewModel(
    Guid Id,
    string Nome,
    decimal Valor
);

public record CadastrarProdutoViewModel(
    [Required(ErrorMessage = "O campo \"Nome\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O campo \"Nome\" deve conter entre 3 e 100 caracteres.")]
    string Nome,

    [Required(ErrorMessage = "O campo \"Valor\" deve ser preenchido.")]
    [Range(0.01, 99999.99, ErrorMessage = "O campo \"Valor\" deve ser maior que zero.")]
    decimal Valor
);

public record EditarProdutoViewModel(
    Guid Id,

    [Required(ErrorMessage = "O campo \"Nome\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O campo \"Nome\" deve conter entre 3 e 100 caracteres.")]
    string Nome,

    [Required(ErrorMessage = "O campo \"Valor\" deve ser preenchido.")]
    [Range(0.01, 99999.99, ErrorMessage = "O campo \"Valor\" deve ser maior que zero.")]
    decimal Valor
);

public record ExcluirProdutoViewModel(
    Guid Id,
    string Nome,
    decimal Valor
);
