using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleDeBar.Infra.Modulos.ModuloGarcom;

public sealed class GarcomConfiguration : IEntityTypeConfiguration<Garcom>
{
    public void Configure(EntityTypeBuilder<Garcom> builder)
    {
        builder.ToTable("TBGarcom");

        builder.HasKey(g => g.Id)
            .HasName("PK_TBGarcom");

        builder.Property(g => g.Id)
            .ValueGeneratedNever();

        builder.Property(g => g.UserId)
            .IsRequired();

        builder.Property(g => g.Nome)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(g => g.Telefone)
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(g => g.Cpf)
            .HasMaxLength(14)
            .IsRequired();

        builder.HasIndex(g => new { g.UserId, g.Cpf })
            .IsUnique()
            .HasDatabaseName("UQ_TBGarcom_UserId_Cpf");

        builder.HasIndex(g => new { g.UserId, g.Telefone })
            .IsUnique()
            .HasDatabaseName("UQ_TBGarcom_UserId_Telefone");
    }
}
