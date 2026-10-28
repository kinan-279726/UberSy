using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.ServiceRequests.RideRequests;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbRideRequestConfigurations : IEntityTypeConfiguration<TbRideRequest>
{
    public void Configure(EntityTypeBuilder<TbRideRequest> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbRideRe__3214EC07B4A5C518");
        builder.ToTable("TbRideRequest");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.DeliveryFee).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.DistanceKm).HasColumnType("decimal(6, 2)");
        builder.Property(e => e.DriverEarning).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.ReceiverName).HasMaxLength(100);
        builder.Property(e => e.ReceiverPhone).HasMaxLength(20);
        builder.HasOne(d => d.Driver).WithMany(p => p.RideRequests)
            .HasForeignKey(d => d.DriverId)
            .HasConstraintName("FK_TbRideRequest_Driver");
        builder.HasOne(d => d.IdNavigation).WithOne(p => p.RideRequest)
            .HasForeignKey<TbRideRequest>(d => d.Id)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbRideRequest_ServiceRequest");
    }
}