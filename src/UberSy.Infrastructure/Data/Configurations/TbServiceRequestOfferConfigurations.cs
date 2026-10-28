using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.ServiceRequests.Offers;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbServiceRequestOfferConfigurations : IEntityTypeConfiguration<TbServiceRequestOffer>
{
    public void Configure(EntityTypeBuilder<TbServiceRequestOffer> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbServic__3214EC07D247AFFA");
        builder.ToTable("TbServiceRequestOffer");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.SentAt).HasDefaultValueSql("(sysutcdatetime())");
        builder.HasOne(d => d.Driver).WithMany(p => p.Offers)
            .HasForeignKey(d => d.DriverId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbServiceRequestOffer_Driver");
        builder.HasOne(d => d.ServiceRequest).WithMany(p => p.Offers)
            .HasForeignKey(d => d.ServiceRequestId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbServiceRequestOffer_ServiceRequest");
        builder.Property<byte[]>("RowVersion").IsRowVersion();
    }
}