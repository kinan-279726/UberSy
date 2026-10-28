namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Dtos;

public sealed class AddOnDto
{
    public Guid Id { get; init; }

    public Guid RestaurantId { get; init; }
   
    public string NameAr { get; init; } = null!;
    
    public string NameEn { get; init; } = null!;
    
    public decimal Price { get; init; }
    
    public bool IsActive { get; init; }
}
