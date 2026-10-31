using NetTopologySuite.Geometries;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Restaurants;

namespace UberSy.Test.Common.Restaurants;

public static class RestaurantFactory
{
    public static Result<TbRestaurant> CreateRestaurant(
    Guid? id = null,
    Guid? ownerId = null,
    string nameAr = "مطعم تجريبي",
    string nameEn = "Test Restaurant",
    string? descriptionAr = "وصف تجريبي",
    string? descriptionEn = "Test description",
    string phone = "+963912345678",
    string? email = "test@restaurant.com",
    decimal commissionRate = 10m,
    decimal taxRate = 5m,
    TimeOnly? openingTime = null,
    TimeOnly? closingTime = null,
    decimal minOrderAmount = 20m,
    decimal maxOrderAmount = 500m,
    double latitude = 33.5138,
    double longitude = 36.2765)
    {
        return TbRestaurant.Create(
            id: Guid.NewGuid(),
            ownerId: Guid.NewGuid(),
            nameAr: nameAr,
            nameEn: nameEn,
            descriptionAr: descriptionAr,
            descriptionEn: descriptionEn,
            phone: phone,
            email: email,
            commissionRate: commissionRate,
            taxRate: taxRate,
            openingTime: openingTime ?? new TimeOnly(9, 0),
            closingTime: closingTime ?? new TimeOnly(23, 0),
            minOrderAmount: minOrderAmount,
            maxOrderAmount: maxOrderAmount,
            boundary: new Point(latitude , longitude)
        );
    }
}
