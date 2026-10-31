using UberSy.Domain.Restaurants.AddOns;
using Xunit;

namespace UberSy.Domain.UnitTests.Restaurants.AddOns;

public class AddOnTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbAddOn CreateValidAddOn()
    {
        return TbAddOn.Create(
            id: Guid.NewGuid(),
            restaurantId: Guid.NewGuid(),
            nameAr: "جبنة إضافية",
            nameEn: "Extra Cheese",
            price: 5.50m
        ).Value;
    }

    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenRestaurantIdIsEmpty()
    {
        var result = TbAddOn.Create(
            id: Guid.NewGuid(),
            restaurantId: Guid.Empty,
            nameAr: "جبنة إضافية",
            nameEn: "Extra Cheese",
            price: 5.50m);

        Assert.False(result.IsSuccess);
        Assert.Equal(AddOnErrors.RestaurantIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenNameArIsTooShort()
    {
        var result = TbAddOn.Create(
            Guid.NewGuid(), Guid.NewGuid(), "A", "Extra Cheese", 5.50m);

        Assert.False(result.IsSuccess);
        Assert.Equal(AddOnErrors.NameArTooShort.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenNameEnIsTooLong()
    {
        var result = TbAddOn.Create(
            Guid.NewGuid(), Guid.NewGuid(), "جبنة", new string('A', 101), 5.50m);

        Assert.False(result.IsSuccess);
        Assert.Equal(AddOnErrors.NameEnTooLong.Code, result.TopError.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ShouldReturnError_WhenPriceIsZeroOrNegative(decimal invalidPrice)
    {
        var result = TbAddOn.Create(
            Guid.NewGuid(), Guid.NewGuid(), "جبنة", "Cheese", invalidPrice);

        Assert.False(result.IsSuccess);
        Assert.Equal(AddOnErrors.PriceZero.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_AndSetIsActiveTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();

        // Act
        var result = TbAddOn.Create(id, restaurantId, "  جبنة  ", "  Cheese  ", 5.50m);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(restaurantId, result.Value.RestaurantId);
        Assert.Equal("جبنة", result.Value.NameAr); // Verify Trim
        Assert.Equal("Cheese", result.Value.NameEn); // Verify Trim
        Assert.Equal(5.50m, result.Value.Price);
        Assert.True(result.Value.IsActive); // Default is true
    }

    // ============================================================
    // Update Tests
    // ============================================================

    [Fact]
    public void Update_ShouldReturnError_WhenProfileIsInvalid()
    {
        var addOn = CreateValidAddOn();

        var result = addOn.Update("", "Cheese", 5.50m);

        Assert.False(result.IsSuccess);
        Assert.Equal(AddOnErrors.NameArRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Update_ShouldReturnError_WhenPriceIsInvalid()
    {
        var addOn = CreateValidAddOn();

        var result = addOn.Update("جبنة", "Cheese", -5m);

        Assert.False(result.IsSuccess);
        Assert.Equal(AddOnErrors.PriceZero.Code, result.TopError.Code);
    }

    [Fact]
    public void Update_ShouldReturnSuccess_AndUpdateProperties()
    {
        // Arrange
        var addOn = CreateValidAddOn();

        // Act
        var result = addOn.Update("لحم إضافي", "Extra Meat", 10.00m);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("لحم إضافي", addOn.NameAr);
        Assert.Equal("Extra Meat", addOn.NameEn);
        Assert.Equal(10.00m, addOn.Price);
    }

    // ============================================================
    // State Transitions (Activate / Deactivate)
    // ============================================================

    [Fact]
    public void Deactivate_ShouldReturnSuccess_AndSetIsActiveFalse()
    {
        var addOn = CreateValidAddOn();
        Assert.True(addOn.IsActive);

        var result = addOn.Deactivate();

        Assert.True(result.IsSuccess);
        Assert.False(addOn.IsActive);
    }

    [Fact]
    public void Deactivate_ShouldReturnError_WhenAlreadyInactive()
    {
        var addOn = CreateValidAddOn();
        addOn.Deactivate(); // First time

        var result = addOn.Deactivate(); // Second time

        Assert.False(result.IsSuccess);
        Assert.Equal(AddOnErrors.AddOnAlreadyInactive.Code, result.TopError.Code);
    }

    [Fact]
    public void Activate_ShouldReturnSuccess_AndSetIsActiveTrue()
    {
        var addOn = CreateValidAddOn();
        addOn.Deactivate(); // Make it inactive first

        var result = addOn.Activate();

        Assert.True(result.IsSuccess);
        Assert.True(addOn.IsActive);
    }

    [Fact]
    public void Activate_ShouldReturnError_WhenAlreadyActive()
    {
        var addOn = CreateValidAddOn(); // Default is active

        var result = addOn.Activate();

        Assert.False(result.IsSuccess);
        Assert.Equal(AddOnErrors.AddOnAlreadyActive.Code, result.TopError.Code);
    }
}
