using UberSy.Domain.Restaurants.Categories;
using Xunit;

namespace UberSy.Domain.UnitTests.Restaurants.Categories;

public class CategoryTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbCategory CreateValidCategory()
    {
        return TbCategory.Create(
            id: Guid.NewGuid(),
            restaurantId: Guid.NewGuid(),
            printerId: null,
            nameAr: "مقبلات",
            nameEn: "Appetizers",
            sortOrder: 1
        ).Value;
    }

    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenRestaurantIdIsEmpty()
    {
        var result = TbCategory.Create(
            id: Guid.NewGuid(),
            restaurantId: Guid.Empty,
            printerId: null,
            nameAr: "مقبلات",
            nameEn: "Appetizers",
            sortOrder: 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(CategoryErrors.RestaurantIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenNameArIsTooShort()
    {
        var result = TbCategory.Create(
            Guid.NewGuid(), Guid.NewGuid(), null, "A", "Appetizers", 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(CategoryErrors.NameArTooShort.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenNameEnIsTooLong()
    {
        var result = TbCategory.Create(
            Guid.NewGuid(), Guid.NewGuid(), null, "مقبلات", new string('A', 101), 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(CategoryErrors.NameEnTooLong.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_AndSetIsActiveTrue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();
        var printerId = Guid.NewGuid();

        // Act
        var result = TbCategory.Create(id, restaurantId, printerId, "  مقبلات  ", "  Appetizers  ", 5);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(restaurantId, result.Value.RestaurantId);
        Assert.Equal(printerId, result.Value.PrinterId);
        Assert.Equal("مقبلات", result.Value.NameAr); // Verify Trim
        Assert.Equal("Appetizers", result.Value.NameEn); // Verify Trim
        Assert.Equal(5, result.Value.SortOrder);
        Assert.True(result.Value.IsActive); // Default is true
    }

    // ============================================================
    // Update Tests
    // ============================================================

    [Fact]
    public void UpdateProfile_ShouldReturnError_WhenInvalid()
    {
        var category = CreateValidCategory();

        var result = category.UpdateProfile("", "Appetizers");

        Assert.False(result.IsSuccess);
        Assert.Equal(CategoryErrors.NameArRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateProfile_ShouldReturnSuccess_AndUpdateProperties()
    {
        var category = CreateValidCategory();

        var result = category.UpdateProfile("أطباق رئيسية", "Main Dishes");

        Assert.True(result.IsSuccess);
        Assert.Equal("أطباق رئيسية", category.NameAr);
        Assert.Equal("Main Dishes", category.NameEn);
    }

    [Fact]
    public void UpdateSortOrder_ShouldReturnSuccess_AndUpdateProperty()
    {
        var category = CreateValidCategory();
        Assert.Equal(1, category.SortOrder);

        var result = category.UpdateSortOrder(10);

        Assert.True(result.IsSuccess);
        Assert.Equal(10, category.SortOrder);
    }

    [Fact]
    public void AssignPrinter_ShouldReturnSuccess_AndSetPrinterId()
    {
        var category = CreateValidCategory();
        var newPrinterId = Guid.NewGuid();
        Assert.Null(category.PrinterId);

        var result = category.AssignPrinter(newPrinterId);

        Assert.True(result.IsSuccess);
        Assert.Equal(newPrinterId, category.PrinterId);
    }

    [Fact]
    public void RemovePrinter_ShouldReturnSuccess_AndSetPrinterIdToNull()
    {
        var category = CreateValidCategory();
        var printerId = Guid.NewGuid();
        category.AssignPrinter(printerId);
        Assert.NotNull(category.PrinterId);

        var result = category.RemovePrinter();

        Assert.True(result.IsSuccess);
        Assert.Null(category.PrinterId);
    }

    // ============================================================
    // State Transitions
    // ============================================================

    [Fact]
    public void Activate_ShouldReturnSuccess_AndSetIsActiveTrue()
    {
        var category = CreateValidCategory();
        category.Deactivate(); // Set to false first
        Assert.False(category.IsActive);

        var result = category.Activate();

        Assert.True(result.IsSuccess);
        Assert.True(category.IsActive);
    }

    [Fact]
    public void Activate_ShouldReturnError_WhenAlreadyActive()
    {
        var category = CreateValidCategory(); // Default is true

        var result = category.Activate();

        Assert.False(result.IsSuccess);
        Assert.Equal(CategoryErrors.CategoryAlreadyActive.Code, result.TopError.Code);
    }

    [Fact]
    public void Deactivate_ShouldReturnSuccess_AndSetIsActiveFalse()
    {
        var category = CreateValidCategory();
        Assert.True(category.IsActive);

        var result = category.Deactivate();

        Assert.True(result.IsSuccess);
        Assert.False(category.IsActive);
    }

    [Fact]
    public void Deactivate_ShouldReturnError_WhenAlreadyInactive()
    {
        var category = CreateValidCategory();
        category.Deactivate(); // Set to false first

        var result = category.Deactivate();

        Assert.False(result.IsSuccess);
        Assert.Equal(CategoryErrors.CategoryAlreadyInactive.Code, result.TopError.Code);
    }
}
