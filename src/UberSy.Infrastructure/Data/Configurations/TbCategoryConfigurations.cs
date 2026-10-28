using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Restaurants.Categories;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbCategoryConfigurations : IEntityTypeConfiguration<TbCategory>
{
    public void Configure(EntityTypeBuilder<TbCategory> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbCatego__3214EC07EC94C04F");
        builder.ToTable("TbCategory");
        builder.HasIndex(e => e.RestaurantId, "IX_TbCategory_RestaurantId");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.NameAr).HasMaxLength(100);
        builder.Property(e => e.NameEn).HasMaxLength(100);
        builder.HasOne(d => d.Printer).WithMany(p => p.TbCategories)
            .HasForeignKey(d => d.PrinterId)
            .HasConstraintName("FK_TbCategory_Printer");
        builder.HasOne(d => d.Restaurant).WithMany(p => p.Categories)
            .HasForeignKey(d => d.RestaurantId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbCategory_Restaurant");
    }
}