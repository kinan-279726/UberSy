using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.ServiceZones;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbServiceZoneConfigurations : IEntityTypeConfiguration<TbServiceZone>
{
    public void Configure(EntityTypeBuilder<TbServiceZone> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbServic__3214EC078865692F");
        builder.ToTable("TbServiceZone");
        //builder.HasIndex(e => e.Boundary, "SIX_TbServiceZone_Boundary");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.City).HasMaxLength(100);
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.NameAr).HasMaxLength(100);
        builder.Property(e => e.NameEn).HasMaxLength(100);
        //builder.HasIndex(e => e.Boundary, "SIX_TbServiceZone_Boundary")
        //.IsSpatial();
    }
}