using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.ServiceRequests.Dtos;

public sealed record CustomerOrderListItemDto(
    Guid ServiceRequestId, int TrackingNumber, ServiceRequestStatus Status,
    decimal TotalAmount, decimal DeliveryFee, decimal PayableAmount,
    Guid RestaurantId, string RestaurantNameAr, string RestaurantNameEn, string? RestaurantLogoUrl,
    int ItemsCount, DateTimeOffset CreatedAtUtc);
