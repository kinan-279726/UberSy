using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.ServiceRequests.History;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbServiceRequestHistoryConfigurations : IEntityTypeConfiguration<TbServiceRequestHistory>
{
    public void Configure(EntityTypeBuilder<TbServiceRequestHistory> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbServic__3214EC07642341B0");
        builder.ToTable("TbServiceRequestHistory");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.CreatedAtUtc).HasDefaultValueSql("(sysutcdatetime())");
        builder.Property(e => e.Note).HasMaxLength(500);
        builder.HasOne(d => d.ServiceRequest).WithMany(p => p.History)
            .HasForeignKey(d => d.ServiceRequestId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbServiceRequestHistory_ServiceRequest");
    }
}