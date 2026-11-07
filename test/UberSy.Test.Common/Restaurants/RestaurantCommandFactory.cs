using UberSy.Application.Features.Restaurants.Commands.ApproveRestaurant;
using UberSy.Application.Features.Restaurants.Commands.CloseRestaurant;
using UberSy.Application.Features.Restaurants.Commands.CreateRestaurant;
using UberSy.Application.Features.Restaurants.Commands.OpenRestaurant;
using UberSy.Application.Features.Restaurants.Commands.RejectRestaurant;
using UberSy.Application.Features.Restaurants.Commands.UpdateCommissionRate;
using UberSy.Application.Features.Restaurants.Commands.UpdateOperatingHours;
using UberSy.Application.Features.Restaurants.Commands.UpdateOrderLimits;
using UberSy.Application.Features.Restaurants.Commands.UpdateRestaurantImages;
using UberSy.Application.Features.Restaurants.Commands.UpdateRestaurantLocation;
using UberSy.Application.Features.Restaurants.Commands.UpdateRestaurantProfile;
using UberSy.Application.Features.Restaurants.Commands.UpdateTaxRate;

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

    // 1. Approve Restaurant
    public static ApproveRestaurantCommand CreateApproveRestaurantCommand(
        Guid? restaurantId = null)
    {
        return new ApproveRestaurantCommand(
            restaurantId ?? Guid.NewGuid());
    }

    // 2. Close Restaurant
    public static CloseRestaurantCommand CreateCloseRestaurantCommand(
        Guid? restaurantId = null)
    {
        return new CloseRestaurantCommand(
            restaurantId ?? Guid.NewGuid());
    }

    // 3. Open Restaurant
    public static OpenRestaurantCommand CreateOpenRestaurantCommand(
        Guid? restaurantId = null)
    {
        return new OpenRestaurantCommand(
            restaurantId ?? Guid.NewGuid());
    }

    // 4. Reject Restaurant
    public static RejectRestaurantCommand CreateRejectRestaurantCommand(
        Guid? restaurantId = null)
    {
        return new RejectRestaurantCommand(
            restaurantId ?? Guid.NewGuid());
    }

    // 5. Update Commission Rate
    public static UpdateCommissionRateCommand CreateUpdateCommissionRateCommand(
        Guid? restaurantId = null,
        decimal? commissionRate = null)
    {
        return new UpdateCommissionRateCommand(
            restaurantId ?? Guid.NewGuid(),
            commissionRate ?? 10m);
    }

    // 6. Update Operating Hours
    public static UpdateOperatingHoursCommand CreateUpdateOperatingHoursCommand(
        Guid? restaurantId = null,
        TimeOnly? openingTime = null,
        TimeOnly? closingTime = null)
    {
        return new UpdateOperatingHoursCommand(
            restaurantId ?? Guid.NewGuid(),
            openingTime ?? new TimeOnly(9, 0),
            closingTime ?? new TimeOnly(23, 0));
    }

    // 7. Update Order Limits
    public static UpdateOrderLimitsCommand CreateUpdateOrderLimitsCommand(
        Guid? restaurantId = null,
        decimal? minOrderAmount = null,
        decimal? maxOrderAmount = null)
    {
        return new UpdateOrderLimitsCommand(
            restaurantId ?? Guid.NewGuid(),
            minOrderAmount ?? 10m,
            maxOrderAmount ?? 500m);
    }

    // 8. Update Restaurant Images
    public static UpdateRestaurantImagesCommand CreateUpdateRestaurantImagesCommand(
        Guid? restaurantId = null,
        string? logoUrl = null,
        string? coverUrl = null)
    {
        return new UpdateRestaurantImagesCommand(
            restaurantId ?? Guid.NewGuid(),
            logoUrl,
            coverUrl);
    }

    // 9. Update Restaurant Location
    public static UpdateRestaurantLocationCommand CreateUpdateRestaurantLocationCommand(
        Guid? restaurantId = null,
        double? latitude = null,
        double? longitude = null)
    {
        return new UpdateRestaurantLocationCommand(
            restaurantId ?? Guid.NewGuid(),
            latitude ?? 33.5138,   // Damascus Latitude
            longitude ?? 36.2765); // Damascus Longitude
    }

    // 10. Update Restaurant Profile
    public static UpdateRestaurantProfileCommand CreateUpdateRestaurantProfileCommand(
        Guid? restaurantId = null,
        string? nameAr = null,
        string? nameEn = null,
        string? descriptionAr = null,
        string? descriptionEn = null,
        string? phone = null,
        string? email = null)
    {
        return new UpdateRestaurantProfileCommand(
            restaurantId ?? Guid.NewGuid(),
            nameAr ?? "مطعم الشام",
            nameEn ?? "Al Sham Restaurant",
            descriptionAr ?? "مأكولات شامية أصيلة",
            descriptionEn ?? "Authentic Syrian food",
            phone ?? "+963912345678",
            email ?? "info@alsham.com");
    }

    // 11. Update Tax Rate
    public static UpdateTaxRateCommand CreateUpdateTaxRateCommand(
        Guid? restaurantId = null,
        decimal? taxRate = null)
    {
        return new UpdateTaxRateCommand(
            restaurantId ?? Guid.NewGuid(),
            taxRate ?? 5m);
    }
}
