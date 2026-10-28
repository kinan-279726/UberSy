namespace UberSy.Application.Features.Admin.Dtos;

public sealed class AdminDashboardDto
{
    public DateOnly From { get; init; }
    public DateOnly To { get; init; }
    public int TotalOrders { get; init; }
    public int PendingOrders { get; init; }
    public int PreparingOrders { get; init; }
    public int OutForDeliveryOrders { get; init; }
    public int DeliveredOrders { get; init; }
    public int CancelledOrders { get; init; }
    public decimal GrossSales { get; init; }
    public decimal PlatformRevenue { get; init; }
    public int OrdersWaitingForDriver { get; init; }
    public int DriversOnline { get; init; }
    public int DriversOnDelivery { get; init; }
    public int DriversWithoutBalance { get; init; }
    public int RestaurantsPendingApproval { get; init; }
    public int DriversPendingApproval { get; init; }
}