using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Complaints;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbComplaintConfigurations : IEntityTypeConfiguration<TbComplaint>
{
    public void Configure(EntityTypeBuilder<TbComplaint> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbCompla__3214EC07140EB01E");
        builder.ToTable("TbComplaint");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.CreatedAtUtc).HasDefaultValueSql("(sysutcdatetime())");
        builder.Property(e => e.Message).HasMaxLength(2000);
        builder.Property(e => e.Subject).HasMaxLength(200);
        builder.HasOne(d => d.ServiceRequest).WithMany(p => p.Complaints)
            .HasForeignKey(d => d.ServiceRequestId)
            .HasConstraintName("FK_TbComplaint_ServiceRequest");
    }
}