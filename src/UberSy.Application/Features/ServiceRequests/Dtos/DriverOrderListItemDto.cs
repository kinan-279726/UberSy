using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.ServiceRequests.Dtos;

public sealed record DriverOrderListItemDto(
    Guid ServiceRequestId, int TrackingNumber, ServiceRequestStatus Status,
    string RestaurantNameAr, string CustomerStreet, string CustomerCity,
    decimal CashToCollect, decimal DeliveryFee, decimal DriverEarning,
    DateTimeOffset CreatedAtUtc);