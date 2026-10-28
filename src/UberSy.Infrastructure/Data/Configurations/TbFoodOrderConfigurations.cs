using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.ServiceRequests.FoodOrders;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbFoodOrderConfigurations : IEntityTypeConfiguration<TbFoodOrder>
{
    public void Configure(EntityTypeBuilder<TbFoodOrder> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbFoodOr__3214EC0715E2F3EE");
        builder.ToTable("TbFoodOrder");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.DeliveryFee).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.DiscountAmount).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.DriverEarning).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.PlatformCommissionFromRestaurant).HasColumnType("decimal(10, 2)");
        builder.HasOne(d => d.Address).WithMany(p => p.FoodOrders)
            .HasForeignKey(d => d.AddressId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbFoodOrder_Address");
        builder.HasOne(d => d.Discount).WithMany(p => p.FoodOrders)
            .HasForeignKey(d => d.DiscountId)
            .HasConstraintName("FK_TbFoodOrder_Discount");
        builder.HasOne(d => d.Driver).WithMany(p => p.FoodOrders)
            .HasForeignKey(d => d.DriverId)
            .HasConstraintName("FK_TbFoodOrder_Driver");
        builder.HasOne(d => d.IdNavigation).WithOne(p => p.FoodOrder)
            .HasForeignKey<TbFoodOrder>(d => d.Id)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbFoodOrder_ServiceRequest");
        builder.HasOne(d => d.Restaurant).WithMany(p => p.FoodOrders)
            .HasForeignKey(d => d.RestaurantId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbFoodOrder_Restaurant");
    }
}