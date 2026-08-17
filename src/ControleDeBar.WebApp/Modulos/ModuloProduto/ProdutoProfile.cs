using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloProduto;
namespace ControleDeBar.WebApp.Modulos.ModuloProduto;

public class ProdutoProfile : Profile
{
    public ProdutoProfile()
    {
        CreateMap<ListarProdutosDto, ListarProdutosViewModel>();
        CreateMap<CadastrarProdutoViewModel, CadastrarProdutoDto>();
        CreateMap<EditarProdutoViewModel, EditarProdutoDto>();
        CreateMap<DetalhesProdutoDto, EditarProdutoViewModel>();
        CreateMap<DetalhesProdutoDto, ExcluirProdutoViewModel>();
    }
}