namespace UberSy.Application.Features.Drivers.Dtos;

public sealed class DriverOfferSummaryDto
{
    public Guid OfferId { get; init; }
    public Guid ServiceRequestId { get; init; }
    public int TrackingNumber { get; init; }
    public string RestaurantNameAr { get; init; } = null!;
    public DateTimeOffset ExpiresAtUtc { get; init; }
    public int SecondsRemaining { get; init; }
}
