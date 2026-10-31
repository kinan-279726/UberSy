using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Drivers.Wallet.Enums;
using UberSy.Domain.ServiceRequests;

namespace UberSy.Domain.Drivers.Wallet;

public sealed class TbDriverWalletTransaction : AuditableEntity
{
    public Guid DriverWalletId { get; private set; }

    public Guid? ServiceRequestId { get; private set; }
    
    public Guid? ProcessedByUserId { get; private set; }
    
    public int TransactionId { get; private set; }
    
    public decimal Amount { get; private set; }
    
    public WalletTransactionType Type { get; private set; }

    public TbDriverWallet DriverWallet { get; private set; } = null!;
    
    public TbServiceRequest? ServiceRequest { get; private set; }

    private TbDriverWalletTransaction()
    { }

    private TbDriverWalletTransaction(Guid id, Guid driverWalletId, Guid? serviceRequestId, Guid? processedByUserId,
        decimal amount, WalletTransactionType type)
        : base(id)
    {
        DriverWalletId = driverWalletId;
        ServiceRequestId = serviceRequestId;
        ProcessedByUserId = processedByUserId;
        Amount = amount;
        Type = type;
    }

    public static Result<TbDriverWalletTransaction> Create(Guid id, Guid driverWalletId, Guid? serviceRequestId,
        Guid? processedByUserId, decimal amount, WalletTransactionType type)
    {
        if (driverWalletId == Guid.Empty) return WalletErrors.WalletNotFound;
        if (amount == 0) return WalletErrors.AmountMustBePositive;
        if (!Enum.IsDefined(type)) return WalletErrors.AmountMustBePositive;

        if (type == WalletTransactionType.TopUp && processedByUserId is null)
            return WalletErrors.TopUpRequiresProcessor;

        if (type is WalletTransactionType.PlatformCommissionDeduction or WalletTransactionType.CancellationCompensation
            && serviceRequestId is null)
            return WalletErrors.DeductionRequiresServiceRequest;

        return new TbDriverWalletTransaction(id, driverWalletId, serviceRequestId, processedByUserId, amount, type);
    }
}
