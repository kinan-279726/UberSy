using UberSy.Domain.Common.Results;
using UberSy.Domain.Restaurants.Images;

namespace UberSy.Test.Common.Restaurants;

public static class RestaurantImageFactory
{
    public static Result<TbRestaurantImage> CreateRestaurantImage(
        Guid? id = null,
        Guid? restaurantId = null,
        string? imageUrl = null,
        byte? sortOrder = null)
    {
        return TbRestaurantImage.Create(
            id ?? Guid.NewGuid(),
            restaurantId ?? Guid.NewGuid(),
            imageUrl ?? "https://cdn.ubersy.com/restaurants/default.jpg",
            sortOrder ?? 1);
    }
}
