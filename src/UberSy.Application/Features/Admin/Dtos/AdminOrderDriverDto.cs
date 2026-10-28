using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Drivers.Enums;

namespace UberSy.Application.Features.Admin.Dtos;

public sealed class AdminOrderDriverDto
{
    public Guid DriverId { get; init; }
    public Guid UserId { get; init; }
    public VehicleType VehicleType { get; init; }
    public string? PlateNumber { get; init; }
    public DriverStatus Status { get; init; }
}
