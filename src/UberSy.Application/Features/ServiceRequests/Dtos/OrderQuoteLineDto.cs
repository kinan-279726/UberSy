namespace UberSy.Application.Features.ServiceRequests.Dtos;

public sealed class OrderQuoteLineDto
{
    public Guid ItemId { get; init; }
    public string ItemNameAr { get; init; } = null!;
    public string ItemNameEn { get; init; } = null!;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public List<string> AddOnNamesAr { get; init; } = [];
    public decimal LineTotal { get; init; }
}