namespace UberSy.Application.Features.ServiceRequests.Dtos;

public sealed record DailyStatDto(DateOnly Date, int DeliveredOrders, decimal GrossSales, decimal NetEarnings);
