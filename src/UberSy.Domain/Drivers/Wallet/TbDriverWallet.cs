using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Drivers.Wallet;

public sealed class TbDriverWallet : AuditableEntity
{
    public Guid DriverId { get; private set; }
    public decimal Balance { get; private set; }

    public TbDriver Driver { get; private set; } = null!;

    private readonly List<TbDriverWalletTransaction> _transactions = [];
    public IReadOnlyCollection<TbDriverWalletTransaction> Transactions => _transactions.AsReadOnly();

    private TbDriverWallet()
    { }

    private TbDriverWallet(Guid id, Guid driverId)
        : base(id)
    {
        DriverId = driverId;
        Balance = 0;
    }

    public static Result<TbDriverWallet> Create(Guid id, Guid driverId)
    {
        if (driverId == Guid.Empty) return DriverErrors.UserIdRequired;

        return new TbDriverWallet(id, driverId);
    }

    public Result<Updated> Deposit(decimal amount)
    {
        if (amount <= 0) return WalletErrors.AmountMustBePositive;

        Balance += amount;

        return Result.Updated;
    }

    public Result<Updated> Deduct(decimal amount)
    {
        if (amount <= 0) return WalletErrors.AmountMustBePositive;

        Balance -= amount;

        return Result.Updated;
    }
}