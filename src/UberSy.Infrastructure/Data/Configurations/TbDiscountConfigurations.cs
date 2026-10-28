using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Discounts;
using UberSy.Domain.Restaurants.Items;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbDiscountConfigurations : IEntityTypeConfiguration<TbDiscount>
{
    public void Configure(EntityTypeBuilder<TbDiscount> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbDiscou__3214EC07F5023E34");
        builder.ToTable("TbDiscount");
        builder.HasIndex(e => e.Code, "UQ_TbDiscount_Code").IsUnique();
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Code).HasMaxLength(20);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.MinOrderAmount).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.Value).HasColumnType("decimal(10, 2)");
        builder.HasOne(d => d.Restaurant).WithMany(p => p.Discounts)
            .HasForeignKey(d => d.RestaurantId)
            .HasConstraintName("FK_TbDiscount_Restaurant");
        builder.HasMany(d => d.Items).WithMany(p => p.Discounts)
            .UsingEntity<Dictionary<string, object>>(
                "TbDiscountItem",
                r => r.HasOne<TbItem>().WithMany()
                    .HasForeignKey("ItemId")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TbDiscountItem_Item"),
                l => l.HasOne<TbDiscount>().WithMany()
                    .HasForeignKey("DiscountId")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TbDiscountItem_Discount"),
                j =>
                {
                    j.HasKey("DiscountId", "ItemId");
                    j.ToTable("TbDiscountItem");
                });
    }
}