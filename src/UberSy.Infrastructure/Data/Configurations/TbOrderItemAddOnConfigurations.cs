using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.ServiceRequests.FoodOrders;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbOrderItemAddOnConfigurations : IEntityTypeConfiguration<TbOrderItemAddOn>
{
    public void Configure(EntityTypeBuilder<TbOrderItemAddOn> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbOrderI__3214EC07C054362B");
        builder.ToTable("TbOrderItemAddOn");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        builder.HasOne(d => d.AddOn).WithMany(p => p.OrderItemAddOns)
            .HasForeignKey(d => d.AddOnId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbOrderItemAddOn_AddOn");
        builder.HasOne(d => d.OrderItem).WithMany(p => p.AddOns)
            .HasForeignKey(d => d.OrderItemId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbOrderItemAddOn_OrderItem");
    }
}