namespace UberSy.Application.Features.ServiceRequests.Dtos;
public sealed class FoodOrderDetailsDto
{
    public Guid ServiceRequestId { get; init; }
    public int TrackingNumber { get; init; }
    public byte Status { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTimeOffset ResponseDeadlineUtc { get; init; }
    public List<FoodOrderLineDto> Items { get; init; } = [];
}
