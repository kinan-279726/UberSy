using UberSy.Domain.Common.Results;
using UberSy.Domain.Restaurants.AddOns;

namespace UberSy.Test.Common.Restaurants;

public static class AddOnFactory
{
    public static Result<TbAddOn> CreateAddOn(
        Guid? id = null,
        Guid? restaurantId = null,
        string? nameAr = null,
        string? nameEn = null,
        decimal? price = null)
    {
        return TbAddOn.Create(
            id ?? Guid.NewGuid(),
            restaurantId ?? Guid.NewGuid(),
            nameAr ?? "جبنة إضافية",
            nameEn ?? "Extra Cheese",
            price ?? 2.5m);
    }
}
