using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Common;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Drivers;
using UberSy.Domain.Drivers.Enums;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbDriverConfigurations : IEntityTypeConfiguration<TbDriver>
{
    //private readonly IDataProtector _protector;

    //public TbDriverConfigurations(IDataProtectionProvider provider)
    //{
    //    _protector = provider.CreateProtector("UberSy.Driver.NationalId");
    //}

    public void Configure(EntityTypeBuilder<TbDriver> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbDriver__3214EC0703EA4F36");
        builder.ToTable("TbDriver");
        builder.HasIndex(e => e.UserId, "UQ_TbDriver_UserId").IsUnique();
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Color).HasMaxLength(100);
        
        builder.Property(e => e.Model).HasMaxLength(100);
        builder.Property(e => e.NationalId).HasMaxLength(20);
        builder.Property(e => e.PhotoUrl).HasMaxLength(1000);
        
        builder.Property(e => e.ServiceTier).HasDefaultValue(ServiceTier.Standard);
        builder.Property(e => e.Status).HasDefaultValue(DriverStatus.Offline);
        builder.Property(e => e.LicenseNumber)
            .HasMaxLength(50)
            //.HasConversion(
            //        v => _protector.Protect(v),
            //        v => _protector.Unprotect(v)
            //)
            ;
        builder.Property(e => e.PlateNumber).HasMaxLength(20)
             //.HasConversion(
             //       v => _protector.Protect(v),
             //       v => _protector.Unprotect(v)
             //)
             ;
    }
}