using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Drivers.Enums;

namespace UberSy.Application.Features.Admin.Dtos;

public sealed class AdminDriverListItemDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public VehicleType VehicleType { get; init; }
    public ServiceTier ServiceTier { get; init; }
    public CarType? CarType { get; init; }
    public string NationalId { get; init; } = null!;
    public string? PlateNumber { get; init; }
    public DriverStatus Status { get; init; }
    public bool IsApproved { get; init; }
    public decimal WalletBalance { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
}
