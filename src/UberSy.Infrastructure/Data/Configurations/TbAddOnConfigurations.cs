using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Restaurants.AddOns;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbAddOnConfigurations : IEntityTypeConfiguration<TbAddOn>
{
    public void Configure(EntityTypeBuilder<TbAddOn> builder)
    {

        builder.HasKey(e => e.Id).HasName("PK__TbAddOn__3214EC070FA8160A");

        builder.ToTable("TbAddOn");

        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.NameAr).HasMaxLength(100);
        builder.Property(e => e.NameEn).HasMaxLength(100);
        builder.Property(e => e.Price).HasColumnType("decimal(10, 2)");

        builder.HasOne(d => d.Restaurant).WithMany(p => p.AddOns)
                .HasForeignKey(d => d.RestaurantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TbAddOn_Restaurant");

    }
}
