using ControleDeBar.Dominio.Modulos.ModuloConta;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ControleDeBar.Infra.Modulos.ModuloConta;

public sealed class ItemPedidoConfiguration : IEntityTypeConfiguration<ItemPedido>
{
    public void Configure(EntityTypeBuilder<ItemPedido> builder)
    {
        builder.ToTable("TBItemPedido");

        builder.HasKey(i => i.Id)
            .HasName("PK_TBItemPedido");

        builder.Property(i => i.Id)
            .ValueGeneratedNever();

        builder.Property(i => i.ContaId)
            .IsRequired();

        builder.Property(i => i.ProdutoId)
            .IsRequired();

        builder.Property(i => i.Quantidade)
            .IsRequired();

        builder.Property(i => i.ValorUnitario)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(i => i.Valor)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne(i => i.Conta)
            .WithMany(c => c.Itens)
            .HasForeignKey(i => i.ContaId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasOne(i => i.Produto)
            .WithMany()
            .HasForeignKey(i => i.ProdutoId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
