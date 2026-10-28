namespace UberSy.Application.Features.ServiceRequests.Dtos;

public sealed class FoodOrderSummaryDto
{
    public Guid ServiceRequestId { get; init; }
    public int TrackingNumber { get; init; }
    public Guid RestaurantId { get; init; }
    public byte Status { get; init; }
    public decimal ItemsTotal { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal DistanceKm { get; init; }
    public decimal EstimatedDeliveryFee { get; init; }
    public decimal EstimatedGrandTotal { get; init; }
    public DateTimeOffset ResponseDeadlineUtc { get; init; }
}