using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.ServiceRequests;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbServiceRequestConfigurations : IEntityTypeConfiguration<TbServiceRequest>
{
    public void Configure(EntityTypeBuilder<TbServiceRequest> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbServic__3214EC07831A20B8");
        builder.ToTable("TbServiceRequest");
        builder.HasIndex(e => new { e.CustomerId, e.Status, e.CreatedAtUtc }, "IX_TbServiceRequest_Customer_Status_CreatedAt").IsDescending(false, false, true);
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.CancellationReason).HasMaxLength(100);
        builder.Property(e => e.TotalAmount).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.TrackingNumber).ValueGeneratedOnAdd();
    }
}