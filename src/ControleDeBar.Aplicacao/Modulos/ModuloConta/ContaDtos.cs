using ControleDeBar.Dominio.Modulos.ModuloConta;

namespace ControleDeBar.Aplicacao.Modulos.ModuloConta;

public record ListarContasDto(
    Guid Id,
    string NomeCliente,
    string IdentificacaoMesa,
    string NomeGarcom,
    DateTime DataAbertura,
    SituacaoConta Situacao,
    decimal ValorTotal
);

public record CadastrarContaDto(
    string NomeCliente,
    Guid MesaId,
    Guid GarcomId
);

public record EditarContaDto(
    Guid Id,
    string NomeCliente,
    Guid MesaId,
    Guid GarcomId
);

public record DetalhesContaDto(
    Guid Id,
    string NomeCliente,
    Guid? MesaId,
    string IdentificacaoMesa,
    Guid? GarcomId,
    string NomeGarcom,
    DateTime DataAbertura,
    SituacaoConta Situacao,
    decimal ValorTotal,
    IReadOnlyList<ItemPedidoContaDto> Pedidos
);

public record ItemPedidoContaDto(
    Guid Id,
    string NomeProduto,
    int Quantidade,
    decimal ValorUnitario,
    decimal ValorTotal
);

public record AdicionarPedidoContaDto(
    Guid ContaId,
    Guid ProdutoId,
    int Quantidade
);

public record AlterarQuantidadeItemPedidoDto(
    Guid ContaId,
    Guid ItemPedidoId,
    int Quantidade
);

public record RemoverItemPedidoDto(
    Guid ContaId,
    Guid ItemPedidoId
);
