using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Reviews;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbReviewConfigurations : IEntityTypeConfiguration<TbReview>
{
    public void Configure(EntityTypeBuilder<TbReview> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbReview__3214EC079BD3F549");
        builder.ToTable("TbReview");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.CreatedAtUtc).HasDefaultValueSql("(sysutcdatetime())");
        builder.Property(e => e.Note).HasMaxLength(1000);
        builder.HasOne(d => d.Driver).WithMany(p => p.Reviews)
            .HasForeignKey(d => d.DriverId)
            .HasConstraintName("FK_TbReview_Driver");
        builder.HasOne(d => d.Restaurant).WithMany(p => p.Reviews)
            .HasForeignKey(d => d.RestaurantId)
            .HasConstraintName("FK_TbReview_Restaurant");
        builder.HasOne(d => d.ServiceRequest).WithMany(p => p.Reviews)
            .HasForeignKey(d => d.ServiceRequestId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbReview_ServiceRequest");
    }
}