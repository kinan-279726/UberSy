using MediatR;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Menu.Items.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Queries.GetItemsByRestaurant;

public sealed record GetItemsByRestaurantQuery(Guid RestaurantId) : ICachedQuery<Result<List<ItemDto>>>
{
    public string CacheKey => $"items{RestaurantId}";

    public string[] Tags => [$"items{RestaurantId}"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(1);
}