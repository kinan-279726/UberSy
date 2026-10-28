using UberSy.Application.Features.ServiceRequests.Offers.Dtos;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.Drivers.Dtos;

public sealed class ActiveDeliveryDto
{
    public Guid ServiceRequestId { get; init; }
    public int TrackingNumber { get; init; }
    public ServiceRequestStatus Status { get; init; }
    public OfferPointDto Restaurant { get; init; } = null!;
    public string? RestaurantPhone { get; init; }
    public OfferPointDto Customer { get; init; } = null!;
    public string? CustomerPhone { get; init; }
    public List<OfferItemDto> Items { get; init; } = [];
    public decimal RestaurantPrice { get; init; }
    public decimal DeliveryFee { get; init; }
    public decimal CashToCollect { get; init; }
    public decimal DriverEarning { get; init; }
}
