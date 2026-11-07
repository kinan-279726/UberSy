using UberSy.Domain.Common.Results;
using UberSy.Domain.Drivers.Wallet;
using UberSy.Domain.Drivers.Wallet.Enums;

namespace UberSy.Test.Common.Driver;

public static class DriverWalletTransactionFactory
{
    public static Result<TbDriverWalletTransaction> CreateDriverWalletTransaction(
        Guid? id = null,
        Guid? driverWalletId = null,
        Guid? serviceRequestId = null,
        Guid? processedByUserId = null,
        decimal? amount = null,
        WalletTransactionType? type = null)
    {
        var txType = type ?? WalletTransactionType.TopUp;

        var processor = txType == WalletTransactionType.TopUp
            ? (processedByUserId ?? Guid.NewGuid())
            : processedByUserId;

        var serviceRequest = txType is WalletTransactionType.PlatformCommissionDeduction
            or WalletTransactionType.CancellationCompensation
                ? (serviceRequestId ?? Guid.NewGuid())
                : serviceRequestId;

        return TbDriverWalletTransaction.Create(
            id ?? Guid.NewGuid(),
            driverWalletId ?? Guid.NewGuid(),
            serviceRequest,
            processor,
            amount ?? 50m,
            txType);
    }
}
