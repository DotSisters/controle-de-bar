using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloConta;

namespace ControleDeBar.WebApp.Modulos.ModuloConta;

public class ContaProfile : Profile
{
    public ContaProfile()
    {
        CreateMap<ListarContasDto, ListarContasViewModel>();
        CreateMap<CadastrarContaViewModel, CadastrarContaDto>();
        CreateMap<EditarContaViewModel, EditarContaDto>();
        CreateMap<DetalhesContaDto, EditarContaViewModel>();
        CreateMap<DetalhesContaDto, ExcluirContaViewModel>();
        CreateMap<DetalhesContaDto, GerenciarContaViewModel>();
        CreateMap<ItemPedidoContaDto, PedidoContaViewModel>();
    }
}
