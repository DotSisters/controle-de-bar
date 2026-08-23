using System.ComponentModel.DataAnnotations;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControleDeBar.WebApp.Modulos.ModuloConta;

public record ListarContasViewModel(
    Guid Id,
    string NomeCliente,
    string IdentificacaoMesa,
    string NomeGarcom,
    DateTime DataAbertura,
    SituacaoConta Situacao,
    decimal ValorTotal,
    string? TempoDesdeUltimoPedido
);

public record CadastrarContaViewModel(
    [Required(ErrorMessage = "O campo \"Nome do Cliente\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O campo \"Nome do Cliente\" deve conter entre 3 e 100 caracteres.")]
    string NomeCliente,

    [Required(ErrorMessage = "O campo \"Mesa\" deve ser preenchido.")]
    Guid MesaId,

    [Required(ErrorMessage = "O campo \"Garçom\" deve ser preenchido.")]
    Guid GarcomId
)
{
    public IEnumerable<SelectListItem> Mesas { get; init; } = [];
    public IEnumerable<SelectListItem> Garcons { get; init; } = [];
}

public record EditarContaViewModel(
    Guid Id,

    [Required(ErrorMessage = "O campo \"Nome do Cliente\" deve ser preenchido.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O campo \"Nome do Cliente\" deve conter entre 3 e 100 caracteres.")]
    string NomeCliente,

    [Required(ErrorMessage = "O campo \"Mesa\" deve ser preenchido.")]
    Guid MesaId,

    [Required(ErrorMessage = "O campo \"Garçom\" deve ser preenchido.")]
    Guid GarcomId
)
{
    public IEnumerable<SelectListItem> Mesas { get; init; } = [];
    public IEnumerable<SelectListItem> Garcons { get; init; } = [];
}

public record ExcluirContaViewModel(
    Guid Id,
    string NomeCliente,
    string IdentificacaoMesa,
    string NomeGarcom,
    DateTime DataAbertura,
    SituacaoConta Situacao,
    decimal ValorTotal
);

public record GerenciarContaViewModel(
    Guid Id,
    string NomeCliente,
    string IdentificacaoMesa,
    string NomeGarcom,
    DateTime DataAbertura,
    SituacaoConta Situacao,
    decimal ValorTotal,
    List<PedidoContaViewModel> Pedidos
);

public record PedidoContaViewModel(
    Guid Id,
    string NomeProduto,
    int Quantidade,
    decimal ValorUnitario,
    decimal ValorTotal
);

public class AdicionarPedidosContaViewModel
{
    public Guid ContaId { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string IdentificacaoMesa { get; set; } = string.Empty;
    public string NomeGarcom { get; set; } = string.Empty;
    public SituacaoConta Situacao { get; set; }
    public decimal ValorTotal { get; set; }

    [Required(ErrorMessage = "O campo \"Produto\" deve ser preenchido.")]
    public Guid ProdutoId { get; set; }

    [Required(ErrorMessage = "O campo \"Quantidade\" deve ser preenchido.")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo \"Quantidade\" deve ser maior que zero.")]
    public int Quantidade { get; set; } = 1;

    public List<PedidoContaViewModel> Pedidos { get; set; } = [];
    public IEnumerable<SelectListItem> Produtos { get; set; } = [];
}
