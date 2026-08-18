using AutoMapper;
using ControleDeBar.Aplicacao.Modulos.ModuloGarcom;

namespace ControleDeBar.WebApp.Modulos.ModuloGarcom;

public class GarcomProfile : Profile
{
    public GarcomProfile()
    {
        CreateMap<ListarGarconsDto, ListarGarconsViewModel>();
        CreateMap<CadastrarGarcomViewModel, CadastrarGarcomDto>();
        CreateMap<EditarGarcomViewModel, EditarGarcomDto>();
        CreateMap<DetalhesGarcomDto, EditarGarcomViewModel>();
        CreateMap<DetalhesGarcomDto, ExcluirGarcomViewModel>();
    }
}
