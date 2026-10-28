using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Restaurants.Printers;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbPrinterConfigurations : IEntityTypeConfiguration<TbPrinter>
{
    public void Configure(EntityTypeBuilder<TbPrinter> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbPrinte__3214EC071CD9E703");
        builder.ToTable("TbPrinter");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.ConnectionInfo).HasMaxLength(1000);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.Name).HasMaxLength(100);
        builder.HasOne(d => d.Restaurant).WithMany(p => p.Printers)
            .HasForeignKey(d => d.RestaurantId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbPrinter_Restaurant");
    }
}