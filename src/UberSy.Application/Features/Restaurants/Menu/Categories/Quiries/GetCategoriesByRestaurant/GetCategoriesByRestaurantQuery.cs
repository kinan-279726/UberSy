using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Menu.Categories.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Quiries.GetCategoriesByRestaurant;

public sealed record GetCategoriesByRestaurantQuery(Guid RestaurantId) : ICachedQuery<Result<List<CategoryDto>>>
{
    public string CacheKey => $"category{RestaurantId}";

    public string[] Tags => [$"category{RestaurantId}"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}