using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Complaints;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbComplaintMessageConfigurations : IEntityTypeConfiguration<TbComplaintMessage>
{
    public void Configure(EntityTypeBuilder<TbComplaintMessage> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbCompla__3214EC075DB39A21");
        builder.ToTable("TbComplaintMessage");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Message).HasMaxLength(2000);
        builder.Property(e => e.CreatedAtUtc).HasDefaultValueSql("(sysutcdatetime())");
        builder.HasOne(d => d.Complaint).WithMany(p => p.Messages)
            .HasForeignKey(d => d.ComplaintId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbComplaintMessage_Complaint");
    }
}