using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Restaurants.Images;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbRestaurantImageConfigurations : IEntityTypeConfiguration<TbRestaurantImage>
{
    public void Configure(EntityTypeBuilder<TbRestaurantImage> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbRestau__3214EC0721745D85");
        builder.ToTable("TbRestaurantImage");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.ImageUrl).HasMaxLength(1000);
        builder.HasOne(d => d.Restaurant).WithMany(p => p.Images)
            .HasForeignKey(d => d.RestaurantId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbRestaurantImage_Restaurant");
    }
}