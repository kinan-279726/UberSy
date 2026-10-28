namespace UberSy.Application.Features.ServiceRequests.Offers.Dtos;

public sealed class OfferItemDto
{
    public string NameAr { get; init; } = null!;
    public string NameEn { get; init; } = null!;
    public int Quantity { get; init; }
    public string? Note { get; init; }
    public List<string> AddOnNamesAr { get; init; } = [];
}