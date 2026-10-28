using MediatR;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Queries.SearchNearbyRestaurants;

public sealed record SearchNearbyRestaurantsQuery(double Latitude, double Longitude, double RadiusKm) : ICachedQuery<Result<List<RestaurantDto>>>
{
    public string CacheKey => "restaurants";

    public string[] Tags => ["restaurant"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(1);
}