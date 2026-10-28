using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Addresses;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbAddressConfigurations : IEntityTypeConfiguration<TbAddress>
{
    public void Configure(EntityTypeBuilder<TbAddress> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbAddres__3214EC07BCC06044");

        builder.ToTable("TbAddress");

        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.City).HasMaxLength(100);
        builder.Property(e => e.Floor).HasMaxLength(100);
        builder.Property(e => e.Label).HasMaxLength(100);
        builder.Property(e => e.Phone).HasMaxLength(20);
        builder.Property(e => e.Street).HasMaxLength(100);
    }
}