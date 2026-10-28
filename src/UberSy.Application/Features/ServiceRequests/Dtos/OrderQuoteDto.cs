namespace UberSy.Application.Features.ServiceRequests.Dtos;

public sealed class OrderQuoteDto
{
    public Guid RestaurantId { get; init; }
    public string RestaurantNameAr { get; init; } = null!;
    public List<OrderQuoteLineDto> Lines { get; init; } = [];
    public decimal ItemsTotal { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TaxRate { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public decimal DistanceKm { get; init; }
    public decimal EstimatedDeliveryFee { get; init; }
    public decimal EstimatedGrandTotal { get; init; }
}
