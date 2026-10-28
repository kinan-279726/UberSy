using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Restaurants.AddOns;
using UberSy.Domain.Restaurants.Items;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbItemConfigurations : IEntityTypeConfiguration<TbItem>
{
    public void Configure(EntityTypeBuilder<TbItem> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbItem__3214EC0723E01AA4");
        builder.ToTable("TbItem");
        builder.HasIndex(e => e.CategoryId, "IX_TbItem_CategoryId");
        builder.HasIndex(e => e.RestaurantId, "IX_TbItem_RestaurantId");
        builder.HasIndex(e => new { e.RestaurantId, e.IsActive }, "IX_TbItem_Restaurant_Active");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.DescriptionAr).HasMaxLength(1000);
        builder.Property(e => e.DescriptionEn).HasMaxLength(1000);
        builder.Property(e => e.ImageUrl).HasMaxLength(1000);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.NameAr).HasMaxLength(100);
        builder.Property(e => e.NameEn).HasMaxLength(100);
        builder.Property(e => e.Price).HasColumnType("decimal(10, 2)");
        builder.HasOne(d => d.Category).WithMany(p => p.Items)
            .HasForeignKey(d => d.CategoryId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbItem_Category");
        builder.HasOne(d => d.Restaurant).WithMany(p => p.Items)
            .HasForeignKey(d => d.RestaurantId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbItem_Restaurant");
        builder.HasMany(d => d.AddOns).WithMany(p => p.Items)
            .UsingEntity<Dictionary<string, object>>(
                "TbItemAddOn",
                r => r.HasOne<TbAddOn>().WithMany()
                    .HasForeignKey("AddOnId")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TbItemAddOn_AddOn"),
                l => l.HasOne<TbItem>().WithMany()
                    .HasForeignKey("ItemId")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TbItemAddOn_Item"),
                j =>
                {
                    j.HasKey("ItemId", "AddOnId");
                    j.ToTable("TbItemAddOn");
                });
    }
}