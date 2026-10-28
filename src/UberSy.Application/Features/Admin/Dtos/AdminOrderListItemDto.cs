using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.Admin.Dtos;

public sealed class AdminOrderListItemDto
{
    public Guid ServiceRequestId { get; init; }
    public int TrackingNumber { get; init; }
    public ServiceRequestStatus Status { get; init; }
    public Guid CustomerId { get; init; }
    public Guid RestaurantId { get; init; }
    public string RestaurantNameAr { get; init; } = null!;
    public Guid? DriverId { get; init; }
    public bool WaitingForDriver { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal DeliveryFee { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
}
