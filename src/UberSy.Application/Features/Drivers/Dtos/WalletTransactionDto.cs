using UberSy.Domain.Drivers.Wallet.Enums;

namespace UberSy.Application.Features.Drivers.Dtos;

public sealed class WalletTransactionDto
{
    public int TransactionId { get; init; }
    public decimal Amount { get; init; }
    public WalletTransactionType Type { get; init; }
    public Guid? ServiceRequestId { get; init; }
    public int? TrackingNumber { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
}
