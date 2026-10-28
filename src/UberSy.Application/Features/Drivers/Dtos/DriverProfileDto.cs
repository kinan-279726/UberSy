using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Drivers.Enums;

namespace UberSy.Application.Features.Drivers.Dtos;

public sealed class DriverProfileDto
{
    public Guid Id { get; init; }
    public VehicleType VehicleType { get; init; }
    public ServiceTier ServiceTier { get; init; }
    public CarType? CarType { get; init; }
    public string? PhotoUrl { get; init; }
    public string? LicenseNumber { get; init; }
    public string? PlateNumber { get; init; }
    public string? Model { get; init; }
    public string? Color { get; init; }
    public DriverStatus Status { get; init; }
    public bool IsApproved { get; init; }
    public decimal WalletBalance { get; init; }
}
