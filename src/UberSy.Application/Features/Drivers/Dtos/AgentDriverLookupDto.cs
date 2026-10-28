using UberSy.Domain.DeliveryRule.Enums;

namespace UberSy.Application.Features.Drivers.Dtos;

public sealed class AgentDriverLookupDto
{
    public Guid DriverId { get; init; }
    public string NameAr { get; init; } = null!;
    public string NameEn { get; init; } = null!;
    public string? Phone { get; init; }
    public string NationalIdMasked { get; init; } = null!;
    public string? PhotoUrl { get; init; }
    public VehicleType VehicleType { get; init; }
    public string? PlateNumber { get; init; }
    public bool IsApproved { get; init; }
    public decimal CurrentBalance { get; init; }
    public bool CanTopUp { get; init; }
}