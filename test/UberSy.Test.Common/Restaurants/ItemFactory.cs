using UberSy.Domain.Common.Results;
using UberSy.Domain.Restaurants.Items;

namespace UberSy.Test.Common.Restaurants;

public static class ItemFactory
{
    public static Result<TbItem> CreateItem(
        Guid? id = null,
        Guid? restaurantId = null,
        Guid? categoryId = null,
        string? nameAr = null,
        string? nameEn = null,
        string? descriptionAr = null,
        string? descriptionEn = null,
        decimal? price = null,
        string? imageUrl = null,
        int? calories = null,
        byte? sortOrder = null)
    {
        return TbItem.Create(
            id ?? Guid.NewGuid(),
            restaurantId ?? Guid.NewGuid(),
            categoryId ?? Guid.NewGuid(),
            nameAr ?? "شاورما دجاج",
            nameEn ?? "Chicken Shawarma",
            descriptionAr ?? "شاورما دجاج طازجة مع صوص الثوم",
            descriptionEn ?? "Fresh chicken shawarma with garlic sauce",
            price ?? 25m,
            imageUrl ?? "https://cdn.ubersy.com/items/default.jpg",
            calories ?? 350,
            sortOrder ?? 1);
    }
}
