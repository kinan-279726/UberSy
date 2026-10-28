using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Restaurants;
using UberSy.Domain.Tags;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbRestaurantConfigurations : IEntityTypeConfiguration<TbRestaurant>
{
    public void Configure(EntityTypeBuilder<TbRestaurant> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbRestau__3214EC071BFEDFB1");
        builder.ToTable("TbRestaurant");
        builder.HasIndex(e => e.OwnerId, "IX_TbRestaurant_OwnerId");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.CommissionRate).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.CoverUrl).HasMaxLength(1000);
        builder.Property(e => e.DescriptionAr).HasMaxLength(1000);
        builder.Property(e => e.DescriptionEn).HasMaxLength(1000);
        builder.Property(e => e.Email).HasMaxLength(100);
        builder.Property(e => e.LogoUrl).HasMaxLength(1000);
        builder.Property(e => e.MaxOrderAmount).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.MinOrderAmount).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.NameAr).HasMaxLength(100);
        builder.Property(e => e.NameEn).HasMaxLength(100);
        builder.Property(e => e.Phone).HasMaxLength(20);
        builder.Property(e => e.Rating).HasColumnType("decimal(3, 2)");
        builder.Property(e => e.TaxRate).HasColumnType("decimal(10, 2)");
        builder.HasMany(d => d.Tags).WithMany(p => p.Restaurants)
            .UsingEntity<Dictionary<string, object>>(
                "TbRestaurantTag",
                r => r.HasOne<TbTag>().WithMany()
                    .HasForeignKey("TagId")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TbRestaurantTag_Tag"),
                l => l.HasOne<TbRestaurant>().WithMany()
                    .HasForeignKey("RestaurantId")
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TbRestaurantTag_Restaurant"),
                j =>
                {
                    j.HasKey("RestaurantId", "TagId");
                    j.ToTable("TbRestaurantTag");
                });
    }
}