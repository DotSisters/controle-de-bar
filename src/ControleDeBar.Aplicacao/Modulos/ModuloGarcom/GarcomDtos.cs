namespace ControleDeBar.Aplicacao.Modulos.ModuloGarcom;

public record ListarGarconsDto(
    Guid Id,
    string Nome,
    string Telefone,
    string Cpf
);

public record CadastrarGarcomDto(
    string Nome,
    string Telefone,
    string Cpf
);

public record EditarGarcomDto(
    Guid Id,
    string Nome,
    string Telefone,
    string Cpf
);

public record DetalhesGarcomDto(
    Guid Id,
    string Nome,
    string Telefone,
    string Cpf
);
