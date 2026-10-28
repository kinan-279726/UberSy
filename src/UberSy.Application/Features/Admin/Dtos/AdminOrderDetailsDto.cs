using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.Admin.Dtos;

public sealed class AdminOrderDetailsDto
{
    public Guid ServiceRequestId { get; init; }
    public int TrackingNumber { get; init; }
    public ServiceRequestStatus Status { get; init; }
    public string? CancellationReason { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
    public AdminOrderRestaurantDto Restaurant { get; init; } = null!;
    public AdminOrderCustomerDto Customer { get; init; } = null!;
    public AdminOrderDriverDto? Driver { get; init; }
    public List<FoodOrderLineDto> Items { get; init; } = [];
    public decimal ItemsTotal { get; init; }
    public decimal DeliveryFee { get; init; }
    public decimal DriverEarning { get; init; }
    public decimal RestaurantCommission { get; init; }
    public decimal PlatformRevenue { get; init; }
    public decimal CashToCollect { get; init; }
    public List<AdminOrderHistoryDto> History { get; init; } = [];
    public List<AdminOrderOfferDto> Offers { get; init; } = [];
}
