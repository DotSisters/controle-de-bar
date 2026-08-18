using ControleDeBar.Dominio.Compartilhado;

namespace ControleDeBar.Dominio.Modulos.ModuloConta;

public interface IRepositorioConta : IRepositorio<Conta>
{
    bool ExisteContaAbertaPorGarcom(Guid garcomId);
    bool ExisteContaAbertaPorMesa(Guid mesaId);
}
