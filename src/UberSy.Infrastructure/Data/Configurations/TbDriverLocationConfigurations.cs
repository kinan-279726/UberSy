using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Drivers;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbDriverLocationConfigurations : IEntityTypeConfiguration<TbDriverLocation>
{
    public void Configure(EntityTypeBuilder<TbDriverLocation> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbDriver__3214EC079E4D59BF");
        builder.ToTable("TbDriverLocation");
        builder.HasIndex(e => e.DriverId, "UQ_TbDriverLocation_DriverId").IsUnique();
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");
        builder.HasOne(d => d.Driver).WithOne(p => p.DriverLocation)
            .HasForeignKey<TbDriverLocation>(d => d.DriverId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbDriverLocation_Driver");
    }
}