namespace UberSy.Application.Features.ServiceRequests.Dtos;

public sealed record RestaurantStatsDto(
    DateOnly From, DateOnly To,
    int TotalOrders, int DeliveredOrders, int CancelledOrders, int InProgressOrders,
    decimal GrossSales, decimal PlatformCommission, decimal NetEarnings, decimal AverageOrderValue,
    List<DailyStatDto> Daily, List<TopItemDto> TopItems);
