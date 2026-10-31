using UberSy.Domain.Restaurants.AddOns;
using UberSy.Domain.Restaurants.Items;
using Xunit;

namespace UberSy.Domain.UnitTests.Restaurants.Items;

public class ItemTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbItem CreateValidItem()
    {
        return TbItem.Create(
            id: Guid.NewGuid(),
            restaurantId: Guid.NewGuid(),
            categoryId: Guid.NewGuid(),
            nameAr: "بيتزا",
            nameEn: "Pizza",
            descriptionAr: "بيتزا بالجبنة",
            descriptionEn: "Cheese Pizza",
            price: 25.00m,
            imageUrl: "https://example.com/pizza.jpg",
            calories: 500,
            sortOrder: 1
        ).Value;
    }

    private static TbAddOn CreateValidAddOn(Guid restaurantId)
    {
        return TbAddOn.Create(
            id: Guid.NewGuid(),
            restaurantId: restaurantId,
            nameAr: "جبنة إضافية",
            nameEn: "Extra Cheese",
            price: 5.00m
        ).Value;
    }

    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenRestaurantIdIsEmpty()
    {
        var result = TbItem.Create(
            Guid.NewGuid(), Guid.Empty, Guid.NewGuid(),
            "بيتزا", "Pizza", "وصف", "Desc", 25m, "url", null, 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(ItemErrors.RestaurantIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenCategoryIdIsEmpty()
    {
        var result = TbItem.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.Empty,
            "بيتزا", "Pizza", "وصف", "Desc", 25m, "url", null, 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(ItemErrors.CategoryIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenNameArIsTooShort()
    {
        var result = TbItem.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "A", "Pizza", "وصف", "Desc", 25m, "url", null, 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(ItemErrors.NameArTooShort.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenDescriptionIsEmpty()
    {
        var result = TbItem.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "بيتزا", "Pizza", "", "Desc", 25m, "url", null, 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(ItemErrors.DescriptionArRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenImageUrlIsEmpty()
    {
        var result = TbItem.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "بيتزا", "Pizza", "وصف", "Desc", 25m, "", null, 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(ItemErrors.ImageUrlRequired.Code, result.TopError.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Create_ShouldReturnError_WhenPriceIsZeroOrNegative(decimal invalidPrice)
    {
        var result = TbItem.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "بيتزا", "Pizza", "وصف", "Desc", invalidPrice, "url", null, 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(ItemErrors.PriceZero.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenCaloriesIsNegative()
    {
        var result = TbItem.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "بيتزا", "Pizza", "وصف", "Desc", 25m, "url", -10, 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(ItemErrors.CaloriesNegative.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_AndSetIsActiveTrue()
    {
        var result = TbItem.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "  بيتزا  ", "  Pizza  ", "  وصف  ", "  Desc  ", 25m, "  url  ", 500, 1);

        Assert.True(result.IsSuccess);
        Assert.Equal("بيتزا", result.Value.NameAr); // Verify Trim
        Assert.Equal("Pizza", result.Value.NameEn);
        Assert.Equal("وصف", result.Value.DescriptionAr);
        Assert.Equal("url", result.Value.ImageUrl);
        Assert.True(result.Value.IsActive); // Default
    }

    // ============================================================
    // Update Tests
    // ============================================================

    [Fact]
    public void UpdatePrice_ShouldReturnError_WhenInvalid()
    {
        var item = CreateValidItem();
        var result = item.UpdatePrice(-1m);

        Assert.False(result.IsSuccess);
        Assert.Equal(ItemErrors.PriceZero.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdatePrice_ShouldReturnSuccess_AndUpdateProperty()
    {
        var item = CreateValidItem();
        var result = item.UpdatePrice(30m);

        Assert.True(result.IsSuccess);
        Assert.Equal(30m, item.Price);
    }

    [Fact]
    public void UpdateCalories_ShouldReturnSuccess_AndUpdateProperty()
    {
        var item = CreateValidItem();
        var result = item.UpdateCalories(600);

        Assert.True(result.IsSuccess);
        Assert.Equal(600, item.Calories);
    }

    [Fact]
    public void UpdateSortOrder_ShouldReturnSuccess_AndUpdateProperty()
    {
        var item = CreateValidItem();
        var result = item.UpdateSortOrder(5);

        Assert.True(result.IsSuccess);
        Assert.Equal(5, item.SortOrder);
    }

    [Fact]
    public void MoveToCategory_ShouldReturnError_WhenCategoryIdIsEmpty()
    {
        var item = CreateValidItem();
        var result = item.MoveToCategory(Guid.Empty);

        Assert.False(result.IsSuccess);
        Assert.Equal(ItemErrors.CategoryIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void MoveToCategory_ShouldReturnSuccess_AndUpdateProperty()
    {
        var item = CreateValidItem();
        var newCategoryId = Guid.NewGuid();
        var result = item.MoveToCategory(newCategoryId);

        Assert.True(result.IsSuccess);
        Assert.Equal(newCategoryId, item.CategoryId);
    }

    // ============================================================
    // State Transitions (Activate / Deactivate)
    // ============================================================

    [Fact]
    public void Deactivate_ShouldReturnSuccess_AndSetIsActiveFalse()
    {
        var item = CreateValidItem();
        var result = item.Deactivate();

        Assert.True(result.IsSuccess);
        Assert.False(item.IsActive);
    }

    [Fact]
    public void Deactivate_ShouldReturnError_WhenAlreadyInactive()
    {
        var item = CreateValidItem();
        item.Deactivate(); // Set to false

        var result = item.Deactivate(); // Try again

        Assert.False(result.IsSuccess);
        Assert.Equal(ItemErrors.ItemAlreadyInactive.Code, result.TopError.Code);
    }

    [Fact]
    public void Activate_ShouldReturnSuccess_AndSetIsActiveTrue()
    {
        var item = CreateValidItem();
        item.Deactivate(); // Set to false

        var result = item.Activate();

        Assert.True(result.IsSuccess);
        Assert.True(item.IsActive);
    }

    // ============================================================
    // AddOns Management
    // ============================================================

    [Fact]
    public void AssignAddOn_ShouldReturnError_WhenAddOnIsFromDifferentRestaurant()
    {
        var item = CreateValidItem();
        var differentRestaurantId = Guid.NewGuid();
        var invalidAddOn = CreateValidAddOn(differentRestaurantId);

        var result = item.AssignAddOn(invalidAddOn);

        Assert.False(result.IsSuccess);
        Assert.Equal(ItemErrors.AddOnFromDifferentRestaurant.Code, result.TopError.Code);
    }

    [Fact]
    public void AssignAddOn_ShouldReturnSuccess_AndAddToList()
    {
        var item = CreateValidItem();
        var validAddOn = CreateValidAddOn(item.RestaurantId);

        var result = item.AssignAddOn(validAddOn);

        Assert.True(result.IsSuccess);
        Assert.Single(item.AddOns);
        Assert.Contains(validAddOn, item.AddOns);
    }

    [Fact]
    public void AssignAddOn_ShouldReturnError_WhenAlreadyAssigned()
    {
        var item = CreateValidItem();
        var validAddOn = CreateValidAddOn(item.RestaurantId);
        item.AssignAddOn(validAddOn); // Assign first time

        var result = item.AssignAddOn(validAddOn); // Assign again

        Assert.False(result.IsSuccess);
        Assert.Equal(ItemErrors.AddOnAlreadyAssigned.Code, result.TopError.Code);
    }

    [Fact]
    public void RemoveAddOn_ShouldReturnError_WhenNotAssigned()
    {
        var item = CreateValidItem();
        var invalidAddOnId = Guid.NewGuid();

        var result = item.RemoveAddOn(invalidAddOnId);

        Assert.False(result.IsSuccess);
        Assert.Equal(ItemErrors.AddOnNotAssigned.Code, result.TopError.Code);
    }

    [Fact]
    public void RemoveAddOn_ShouldReturnSuccess_AndRemoveFromList()
    {
        var item = CreateValidItem();
        var validAddOn = CreateValidAddOn(item.RestaurantId);
        item.AssignAddOn(validAddOn);
        Assert.Single(item.AddOns);

        var result = item.RemoveAddOn(validAddOn.Id);

        Assert.True(result.IsSuccess);
        Assert.Empty(item.AddOns);
    }
}
