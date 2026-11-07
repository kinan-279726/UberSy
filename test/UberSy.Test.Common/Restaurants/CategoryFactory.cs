using UberSy.Domain.Common.Results;
using UberSy.Domain.Restaurants.Categories;

namespace UberSy.Test.Common.Restaurants;

public static class CategoryFactory
{
    public static Result<TbCategory> CreateCategory(
        Guid? id = null,
        Guid? restaurantId = null,
        Guid? printerId = null,
        string? nameAr = null,
        string? nameEn = null,
        byte? sortOrder = null)
    {
        return TbCategory.Create(
            id ?? Guid.NewGuid(),
            restaurantId ?? Guid.NewGuid(),
            printerId,
            nameAr ?? "المشاوي",
            nameEn ?? "Grills",
            sortOrder ?? 1);
    }
}
