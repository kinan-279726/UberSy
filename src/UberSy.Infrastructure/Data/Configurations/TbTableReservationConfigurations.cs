using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Reservations;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbTableReservationConfigurations : IEntityTypeConfiguration<TbTableReservation>
{
    public void Configure(EntityTypeBuilder<TbTableReservation> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbTableR__3214EC07DB205D72");
        builder.ToTable("TbTableReservation");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.DepositAmount).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.Note).HasMaxLength(1000);
        builder.HasOne(d => d.IdNavigation).WithOne(p => p.TableReservation)
            .HasForeignKey<TbTableReservation>(d => d.Id)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbTableReservation_ServiceRequest");
        builder.HasOne(d => d.Restaurant).WithMany(p => p.TableReservations)
            .HasForeignKey(d => d.RestaurantId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbTableReservation_Restaurant");
    }
}