using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.ServiceRequests.Dtos;

public sealed record RestaurantOrderListItemDto(
    Guid ServiceRequestId,
    int TrackingNumber,
    ServiceRequestStatus Status,
    decimal TotalAmount,
    int ItemsCount,
    DateTimeOffset CreatedAtUtc);
