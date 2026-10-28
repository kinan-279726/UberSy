using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Drivers.Wallet;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbDriverWalletConfigurations : IEntityTypeConfiguration<TbDriverWallet>
{
    public void Configure(EntityTypeBuilder<TbDriverWallet> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbDriver__3214EC07F451DB98");
        builder.ToTable("TbDriverWallet");
        builder.HasIndex(e => e.DriverId, "UQ_TbDriverWallet_DriverId").IsUnique();
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Balance).HasColumnType("decimal(10, 2)");
        builder.Property(e => e.CreatedAtUtc).HasDefaultValueSql("(sysutcdatetime())");
        builder.HasOne(d => d.Driver).WithOne(p => p.DriverWallet)
            .HasForeignKey<TbDriverWallet>(d => d.DriverId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbDriverWallet_Driver");
    }
}