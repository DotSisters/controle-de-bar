using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using FluentResults;

namespace ControleDeBar.Aplicacao.Modulos.ModuloProduto;

public class ServicoProduto : ServicoBase<Produto>
{
    private readonly IRepositorioProduto repositorioProduto;

    public ServicoProduto(IRepositorioProduto repositorioProduto)
    {
        this.repositorioProduto = repositorioProduto;
    }

    public Result Cadastrar(CadastrarProdutoDto dto)
    {
        if (ExisteProdutoComMesmoNome(dto.Nome))
            return Falha(nameof(dto.Nome), "Já existe um produto com este nome.");

        Produto novoProduto = new Produto(dto.Nome, dto.Valor);

        Result resultadoValidacao = ValidarEntidade(novoProduto);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioProduto.Cadastrar(novoProduto);

        return Result.Ok();
    }

    public Result Editar(EditarProdutoDto dto)
    {
        if (ExisteProdutoComMesmoNome(dto.Nome, dto.Id))
            return Falha(nameof(dto.Nome), "Já existe um produto com este nome.");

        Produto produtoAtualizado = new Produto(dto.Nome, dto.Valor);

        Result resultadoValidacao = ValidarEntidade(produtoAtualizado);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        bool conseguiuEditar = repositorioProduto.Editar(dto.Id, produtoAtualizado);

        if (!conseguiuEditar)
            return Falha(string.Empty, "Produto não encontrado.");

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Produto? produto = repositorioProduto.SelecionarPorId(id);

        if (produto == null)
            return Falha(string.Empty, "Produto não encontrado.");

        repositorioProduto.Excluir(id);

        return Result.Ok();
    }

    public List<ListarProdutosDto> SelecionarTodos()
    {
        return repositorioProduto
            .SelecionarTodos()
            .Select(p => new ListarProdutosDto(
                p.Id,
                p.Nome,
                p.Valor
            ))
            .ToList();
    }

    public Result<DetalhesProdutoDto> SelecionarPorId(Guid id)
    {
        Produto? produto = repositorioProduto.SelecionarPorId(id);

        if (produto == null)
            return Result.Fail("Produto não encontrado.");

        return Result.Ok(new DetalhesProdutoDto(
            produto.Id,
            produto.Nome,
            produto.Valor
        ));
    }

    private bool ExisteProdutoComMesmoNome(string nome, Guid? idIgnorado = null)
    {
        string nomeNormalizado = NormalizarNome(nome);

        return repositorioProduto
            .SelecionarTodos()
            .Any(p =>
                p.Id != idIgnorado &&
                NormalizarNome(p.Nome) == nomeNormalizado
            );
    }

    private static string NormalizarNome(string nome)
    {
        return nome.Trim().ToLowerInvariant();
    }
}
