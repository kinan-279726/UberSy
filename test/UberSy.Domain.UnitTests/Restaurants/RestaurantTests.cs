using NetTopologySuite.Geometries;
using UberSy.Domain.Restaurants;
using Xunit;

namespace UberSy.Domain.UnitTests.Restaurants;

public class RestaurantTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static Point CreateValidBoundary()
    {
        return new Point(36.2765, 33.5138) { SRID = 4326 };
    }

    private static TbRestaurant CreateValidRestaurant()
    {
        return TbRestaurant.Create(
            id: Guid.NewGuid(),
            ownerId: Guid.NewGuid(),
            nameAr: "مطعم الشام",
            nameEn: "Al-Sham Restaurant",
            descriptionAr: "وصف تجريبي",
            descriptionEn: "Test description",
            phone: "+963912345678",
            email: "test@restaurant.com",
            commissionRate: 10m,
            taxRate: 5m,
            openingTime: new TimeOnly(9, 0),
            closingTime: new TimeOnly(23, 0),
            minOrderAmount: 20m,
            maxOrderAmount: 500m,
            boundary: CreateValidBoundary()
        ).Value;
    }

    // ============================================================
    // Create Tests - General
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenOwnerIdIsEmpty()
    {
        var result = TbRestaurant.Create(
            Guid.NewGuid(), Guid.Empty, "مطعم", "Rest", null, null, "+963912345678", null, 10m, 5m,
            new TimeOnly(9, 0), new TimeOnly(22, 0), 10m, 100m, CreateValidBoundary());

        Assert.False(result.IsSuccess);
        Assert.Equal(RestaurantErrors.OwnerIdRequired.Code, result.TopError.Code);
    }

    // ============================================================
    // Create Tests - Profile Validation
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenNameArIsShort()
    {
        var result = TbRestaurant.Create(
            Guid.NewGuid(), Guid.NewGuid(), "A", "Restaurant", null, null, "+963912345678", null, 10m, 5m,
            new TimeOnly(9, 0), new TimeOnly(22, 0), 10m, 100m, CreateValidBoundary());

        Assert.False(result.IsSuccess);
        Assert.Equal(RestaurantErrors.NameArTooShort.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenPhoneIsInvalid()
    {
        var result = TbRestaurant.Create(
            Guid.NewGuid(), Guid.NewGuid(), "مطعم", "Restaurant", null, null, "0999999999", null, 10m, 5m,
            new TimeOnly(9, 0), new TimeOnly(22, 0), 10m, 100m, CreateValidBoundary());

        Assert.False(result.IsSuccess);
        Assert.Equal(RestaurantErrors.PhoneInvalid.Code, result.TopError.Code);
    }

    // ============================================================
    // Create Tests - Rates, Hours, Limits, Boundary
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenClosingTimeIsBeforeOpening()
    {
        var result = TbRestaurant.Create(
            Guid.NewGuid(), Guid.NewGuid(), "مطعم", "Restaurant", null, null, "+963912345678", null, 10m, 5m,
            new TimeOnly(22, 0), new TimeOnly(9, 0), 10m, 100m, CreateValidBoundary());

        Assert.False(result.IsSuccess);
        Assert.Equal(RestaurantErrors.ClosingTimeMustBeAfterOpeningTime.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenMaxOrderIsLessThanMin()
    {
        var result = TbRestaurant.Create(
            Guid.NewGuid(), Guid.NewGuid(), "مطعم", "Restaurant", null, null, "+963912345678", null, 10m, 5m,
            new TimeOnly(9, 0), new TimeOnly(22, 0), 100m, 50m, CreateValidBoundary());

        Assert.False(result.IsSuccess);
        Assert.Equal(RestaurantErrors.MaxOrderAmountMustBeGreaterThanMin.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_AndSetDefaults()
    {
        var result = TbRestaurant.Create(
            Guid.NewGuid(), Guid.NewGuid(), "  مطعم  ", "  Restaurant  ", null, null, "+963912345678", null, 10m, 5m,
            new TimeOnly(9, 0), new TimeOnly(22, 0), 10m, 100m, CreateValidBoundary());

        Assert.True(result.IsSuccess);
        Assert.Equal("مطعم", result.Value.NameAr); // Verify Trim()
        Assert.False(result.Value.IsOpenNow);       // Default
        Assert.False(result.Value.IsApproved);      // Default
        Assert.False(result.Value.AcceptReservation); // Default
        Assert.Equal(0m, result.Value.Rating);      // Default
    }

    // ============================================================
    // Update Tests
    // ============================================================

    [Fact]
    public void UpdateCommissionRate_ShouldReturnError_WhenExceedsMaximum()
    {
        var restaurant = CreateValidRestaurant();
        var result = restaurant.UpdateCommissionRate(101m);

        Assert.False(result.IsSuccess);
        Assert.Equal(RestaurantErrors.CommissionRateExceedsMaximum.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateOperatingHours_ShouldReturnError_WhenInvalid()
    {
        var restaurant = CreateValidRestaurant();
        var result = restaurant.UpdateOperatingHours(new TimeOnly(20, 0), new TimeOnly(8, 0));

        Assert.False(result.IsSuccess);
        Assert.Equal(RestaurantErrors.ClosingTimeMustBeAfterOpeningTime.Code, result.TopError.Code);
    }

    // ============================================================
    // State Transitions (Approve/Reject/Open/Close)
    // ============================================================

    [Fact]
    public void Approve_ShouldReturnSuccess_AndSetIsApprovedTrue()
    {
        var restaurant = CreateValidRestaurant();
        var result = restaurant.Approve();

        Assert.True(result.IsSuccess);
        Assert.True(restaurant.IsApproved);
    }

    [Fact]
    public void Approve_ShouldReturnError_WhenAlreadyApproved()
    {
        var restaurant = CreateValidRestaurant();
        restaurant.Approve(); // First time
        var result = restaurant.Approve(); // Second time

        Assert.False(result.IsSuccess);
        Assert.Equal(RestaurantErrors.RestaurantAlreadyApproved.Code, result.TopError.Code);
    }

    [Fact]
    public void Open_ShouldReturnError_WhenNotApproved()
    {
        var restaurant = CreateValidRestaurant();
        // Not approved yet
        var result = restaurant.Open();

        Assert.False(result.IsSuccess);
        Assert.Equal(RestaurantErrors.RestaurantNotApproved.Code, result.TopError.Code);
    }

    [Fact]
    public void Open_ShouldReturnSuccess_WhenApproved()
    {
        var restaurant = CreateValidRestaurant();
        restaurant.Approve(); // Approve first

        var result = restaurant.Open();

        Assert.True(result.IsSuccess);
        Assert.True(restaurant.IsOpenNow);
    }

    [Fact]
    public void Close_ShouldReturnError_WhenAlreadyClosed()
    {
        var restaurant = CreateValidRestaurant();
        // Default is closed (false)
        var result = restaurant.Close();

        Assert.False(result.IsSuccess);
        Assert.Equal(RestaurantErrors.RestaurantAlreadyClosed.Code, result.TopError.Code);
    }

    // ============================================================
    // Reservations & Rating
    // ============================================================

    [Fact]
    public void EnableReservation_ShouldSetAcceptReservationTrue()
    {
        var restaurant = CreateValidRestaurant();
        var result = restaurant.EnableReservation();

        Assert.True(result.IsSuccess);
        Assert.True(restaurant.AcceptReservation);
    }

    [Fact]
    public void UpdateRating_ShouldReturnError_WhenGreaterThan5()
    {
        var restaurant = CreateValidRestaurant();
        var result = restaurant.UpdateRating(5.1m);

        Assert.False(result.IsSuccess);
        Assert.Equal(RestaurantErrors.RatingExceedsMaximum.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateRating_ShouldReturnSuccess_WhenValid()
    {
        var restaurant = CreateValidRestaurant();
        var result = restaurant.UpdateRating(4.5m);

        Assert.True(result.IsSuccess);
        Assert.Equal(4.5m, restaurant.Rating);
    }
}
