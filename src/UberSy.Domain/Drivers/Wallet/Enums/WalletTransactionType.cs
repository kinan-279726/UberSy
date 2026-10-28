namespace UberSy.Domain.Drivers.Wallet.Enums;

public enum WalletTransactionType : byte
{
    TopUp = 1,
    PlatformCommissionDeduction = 2,
    CancellationCompensation = 3,
}