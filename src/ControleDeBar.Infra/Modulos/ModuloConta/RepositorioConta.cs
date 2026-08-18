using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Infra.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Infra.Modulos.ModuloConta;

public sealed class RepositorioConta(
    ControleDeBarDbContext dbContext
) : RepositorioBase<Conta>(dbContext), IRepositorioConta
{
    public override Conta? SelecionarPorId(Guid idSelecionado)
    {
        return registros
            .Include(c => c.Mesa)
            .Include(c => c.Garcom)
            .SingleOrDefault(c => c.Id == idSelecionado);
    }

    public override List<Conta> SelecionarTodos()
    {
        return registros
            .Include(c => c.Mesa)
            .Include(c => c.Garcom)
            .ToList();
    }

    public bool ExisteContaAbertaPorGarcom(Guid garcomId)
    {
        return registros.Any(c =>
            c.GarcomId == garcomId &&
            c.Situacao == SituacaoConta.Aberta
        );
    }

    public bool ExisteContaAbertaPorMesa(Guid mesaId)
    {
        return registros.Any(c =>
            c.MesaId == mesaId &&
            c.Situacao == SituacaoConta.Aberta
        );
    }
}
