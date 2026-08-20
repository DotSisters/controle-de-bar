using ControleDeBar.Dominio.Modulos.ModuloConta;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleDeBar.Infra.Modulos.ModuloConta;

public sealed class ContaConfiguration : IEntityTypeConfiguration<Conta>
{
    public void Configure(EntityTypeBuilder<Conta> builder)
    {
        builder.ToTable("TBConta");

        builder.HasKey(c => c.Id)
            .HasName("PK_TBConta");

        builder.Property(c => c.Id)
            .ValueGeneratedNever();

        builder.Property(c => c.UserId)
            .IsRequired();

        builder.Property(c => c.NomeCliente)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.NomeGarcom)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.IdentificacaoMesa)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.DataAbertura)
            .IsRequired();

        builder.Property(c => c.Situacao)
            .IsRequired();

        builder.Property(c => c.ValorTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.MesaId);

        builder.Property(c => c.GarcomId);

        builder.HasOne(c => c.Mesa)
            .WithMany()
            .HasForeignKey(c => c.MesaId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        builder.HasOne(c => c.Garcom)
            .WithMany()
            .HasForeignKey(c => c.GarcomId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}
