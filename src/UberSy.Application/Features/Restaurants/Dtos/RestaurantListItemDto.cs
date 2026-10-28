namespace UberSy.Application.Features.Restaurants.Dtos;

public sealed class RestaurantListItemDto
{
    public Guid Id { get; init; }
    public string NameAr { get; init; } = null!;
    public string NameEn { get; init; } = null!;
    public string? LogoUrl { get; init; }
    public bool IsOpenNow { get; init; }
    public decimal Rating { get; init; }
}