namespace UberSy.Application.Features.ServiceRequests.Offers.Dtos;

public sealed class PendingOfferDto
{
    public Guid? OfferId { get; init; }
    public Guid ServiceRequestId { get; init; }
    public int TrackingNumber { get; init; }
    public DateTimeOffset ExpiresAtUtc { get; init; }
    public int SecondsRemaining { get; init; }

    public OfferPointDto Restaurant { get; init; } = null!;   
    public OfferPointDto? Customer { get; init; } = null!;
    public decimal DistanceKm { get; init; }

    public List<OfferItemDto> Items { get; init; } = [];

    public decimal RestaurantPrice { get; init; }
    public decimal DeliveryFee { get; init; }  
    public decimal CustomerPrice { get; init; }
    public decimal? DriverEarning { get; init; }
}
