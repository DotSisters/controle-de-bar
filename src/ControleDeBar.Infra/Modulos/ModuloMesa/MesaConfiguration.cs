using ControleDeBar.Dominio.Modulos.ModuloMesa;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleDeBar.Infra.Modulos.ModuloMesa;

public sealed class MesaConfiguration : IEntityTypeConfiguration<Mesa>
{
    public void Configure(EntityTypeBuilder<Mesa> builder)
    {
        builder.ToTable("TBMesa");

        builder.HasKey(m => m.Id)
            .HasName("PK_TBMesa");

        builder.Property(m => m.Id)
            .ValueGeneratedNever();

        builder.Property(m => m.UserId)
            .IsRequired();

        builder.Property(m => m.Identificacao)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.QuantidadeLugar)
            .IsRequired();

        builder.Property(m => m.StatusMesa)
            .IsRequired();

        builder.HasIndex(m => new { m.UserId, m.Identificacao })
            .IsUnique()
            .HasDatabaseName("UQ_TBMesa_UserId_Identificacao");
    }
}
