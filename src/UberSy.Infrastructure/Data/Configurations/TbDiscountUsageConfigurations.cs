using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Discounts;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbDiscountUsageConfigurations : IEntityTypeConfiguration<TbDiscountUsage>
{
    public void Configure(EntityTypeBuilder<TbDiscountUsage> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbDiscou__3214EC07A1000626");
        builder.ToTable("TbDiscountUsage");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.UsedAt).HasDefaultValueSql("(sysutcdatetime())");
        builder.HasOne(d => d.Discount).WithMany(p => p.Usages)
            .HasForeignKey(d => d.DiscountId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbDiscountUsage_Discount");
        builder.HasOne(d => d.ServiceRequest).WithMany(p => p.DiscountUsages)
            .HasForeignKey(d => d.ServiceRequestId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbDiscountUsage_ServiceRequest");
    }
}