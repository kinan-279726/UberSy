using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Payments;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbPaymentConfigurations : IEntityTypeConfiguration<TbPayment>
{
    public void Configure(EntityTypeBuilder<TbPayment> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbPaymen__3214EC0790777B2D");
        builder.ToTable("TbPayment");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Amount).HasColumnType("decimal(12, 2)");
        builder.Property(e => e.CreatedAtUtc).HasDefaultValueSql("(sysutcdatetime())");
        builder.Property(e => e.TransactionId).HasMaxLength(100);
        builder.HasOne(d => d.ServiceRequest).WithMany(p => p.Payments)
            .HasForeignKey(d => d.ServiceRequestId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbPayment_ServiceRequest");
    }
}