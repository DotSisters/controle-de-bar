using ControleDeBar.Dominio.Modulos.ModuloProduto;

namespace ControleDeBar.Aplicacao.Modulos.ModuloProduto;

public record ListarProdutosDto(
    Guid Id,
    string Nome,
    decimal Valor
);

public record CadastrarProdutoDto(
    string Nome,
    decimal Valor
);

public record EditarProdutoDto(
    Guid Id,
    string Nome,
    decimal Valor
);

public record DetalhesProdutoDto(
    Guid Id,
    string Nome,
    decimal Valor
);