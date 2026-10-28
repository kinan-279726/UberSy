using UberSy.Application.Features.Restaurants.Menu.Categories.Dtos;

namespace UberSy.Application.Features.Restaurants.Dtos;

public sealed class RestaurantDto
{
    public Guid Id { get; init; }

    public string NameAr { get; init; } = null!;
   
    public string NameEn { get; init; } = null!;
    
    public string Phone { get; init; } = null!;
    
    public bool IsApproved { get; init; }
    
    public bool IsOpenNow { get; init; }
    
    public decimal Rating { get; init; }

    public List<CategoryDto> Categories { get; set; } = [];
}