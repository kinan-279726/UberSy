namespace UberSy.Application.Features.Restaurants.Menu.Categories.Dtos;

public sealed class CategoryDto
{
    public Guid Id { get; init; }

    public Guid RestaurantId { get; init; }

    public Guid? PrinterId { get; init; }

    public string NameAr { get; init; } = null!;

    public string NameEn { get; init; } = null!;

    public bool IsActive { get; init; }

    public byte SortOrder { get; init; }
}
