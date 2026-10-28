using UberSy.Application.Features.Restaurants.Menu.AddOns.Dtos;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Dtos;

public sealed class ItemDto
{
    public Guid Id { get; init; }
    
    public Guid RestaurantId { get; init; }
    
    public Guid CategoryId { get; init; }
    
    public string NameAr { get; init; } = null!;
    
    public string NameEn { get; init; } = null!;
    
    public string DescriptionAr { get; init; } = null!;
    
    public string DescriptionEn { get; init; } = null!;
    
    public decimal Price { get; init; }
    
    public string ImageUrl { get; init; } = null!;
    
    public int? Calories { get; init; }
    
    public bool IsActive { get; init; }
    
    public byte SortOrder { get; init; }

    public List<AddOnDto> AddOns { get; set; } = [];
}
