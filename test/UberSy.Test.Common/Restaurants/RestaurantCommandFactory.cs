using UberSy.Application.Features.Restaurants.Commands.CreateRestaurant;

namespace UberSy.Test.Common.Restaurants;

public static class RestaurantCommandFactory
{
    public static CreateRestaurantCommand CreateRestaurantCommand(
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
        return new CreateRestaurantCommand(
            NameAr: nameAr,
            NameEn: nameEn,
            DescriptionAr: descriptionAr,
            DescriptionEn: descriptionEn,
            Phone: phone,
            Email: email,
            CommissionRate: commissionRate,
            TaxRate: taxRate,
            OpeningTime: openingTime ?? new TimeOnly(9, 0),
            ClosingTime: closingTime ?? new TimeOnly(23, 0),
            MinOrderAmount: minOrderAmount,
            MaxOrderAmount: maxOrderAmount,
            Latitude: latitude,
            Longitude: longitude
        );
    }
}
