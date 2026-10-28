using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UberSy.Domain.Drivers.Wallet;

namespace UberSy.Infrastructure.Data.Configurations;

public class TbDriverWalletTransactionConfigurations : IEntityTypeConfiguration<TbDriverWalletTransaction>
{
    public void Configure(EntityTypeBuilder<TbDriverWalletTransaction> builder)
    {
        builder.HasKey(e => e.Id).HasName("PK__TbDriver__3214EC079D1AF900");
        builder.ToTable("TbDriverWalletTransaction");
        builder.Property(e => e.Id).ValueGeneratedNever();
        builder.Property(e => e.Amount).HasColumnType("decimal(12, 2)");
        builder.Property(e => e.CreatedAtUtc).HasDefaultValueSql("(sysutcdatetime())");
        builder.Property(e => e.TransactionId).ValueGeneratedOnAdd();
        builder.HasOne(d => d.DriverWallet).WithMany(p => p.Transactions)
            .HasForeignKey(d => d.DriverWalletId)
            .OnDelete(DeleteBehavior.ClientSetNull)
            .HasConstraintName("FK_TbDriverWalletTransaction_Wallet");
        builder.HasOne(d => d.ServiceRequest).WithMany(p => p.WalletTransactions)
            .HasForeignKey(d => d.ServiceRequestId)
            .HasConstraintName("FK_TbDriverWalletTransaction_ServiceRequest");
    }
}