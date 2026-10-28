using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.DeliveryRule;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbDeliveryPricingRuleConfigurations : IEntityTypeConfiguration<TbDeliveryPricingRule>
{
    public void Configure(EntityTypeBuilder<TbDeliveryPricingRule> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbDelive__3214EC0737513A98");
        builder.ToTable("TbDeliveryPricingRule");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.CustomerRatePerKm).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.DriverRatePerKm).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.DriverRatePerOrder).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.IsActive).HasDefaultValue(true);
        builder.Property(e => e.MaxDistanceKm).HasColumnType("decimal(6, 2)");
    }
}