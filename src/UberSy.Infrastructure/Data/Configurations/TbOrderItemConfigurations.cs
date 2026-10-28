using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.ServiceRequests.FoodOrders;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbOrderItemConfigurations : IEntityTypeConfiguration<TbOrderItem>
{
    public void Configure(EntityTypeBuilder<TbOrderItem> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbOrderI__3214EC075C0837FA");
        builder.ToTable("TbOrderItem");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Note).HasMaxLength(100);
        builder.Property(e => e.UnitPrice).HasColumnType("decimal(10, 2)");
        builder.HasOne(d => d.FoodOrder).WithMany(p => p.OrderItems)
            .HasForeignKey(d => d.FoodOrderId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbOrderItem_FoodOrder");
        builder.HasOne(d => d.Item).WithMany(p => p.OrderItems)
            .HasForeignKey(d => d.ItemId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbOrderItem_Item");
    }
}