using UberSy.Domain.Restaurants.Images;
using Xunit;

namespace UberSy.Domain.UnitTests.Restaurants.Images;

public class RestaurantImageTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbRestaurantImage CreateValidImage()
    {
        return TbRestaurantImage.Create(
            id: Guid.NewGuid(),
            restaurantId: Guid.NewGuid(),
            imageUrl: "https://example.com/image.jpg",
            sortOrder: 1
        ).Value;
    }

    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenRestaurantIdIsEmpty()
    {
        var result = TbRestaurantImage.Create(
            id: Guid.NewGuid(),
            restaurantId: Guid.Empty,
            imageUrl: "https://example.com/image.jpg",
            sortOrder: 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(RestaurantImageErrors.RestaurantIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenImageUrlIsEmpty()
    {
        var result = TbRestaurantImage.Create(
            Guid.NewGuid(), Guid.NewGuid(), "", 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(RestaurantImageErrors.ImageUrlRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenImageUrlIsWhiteSpace()
    {
        var result = TbRestaurantImage.Create(
            Guid.NewGuid(), Guid.NewGuid(), "   ", 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(RestaurantImageErrors.ImageUrlRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenImageUrlIsTooLong()
    {
        var result = TbRestaurantImage.Create(
            Guid.NewGuid(), Guid.NewGuid(), new string('a', 1001), 1);

        Assert.False(result.IsSuccess);
        Assert.Equal(RestaurantImageErrors.ImageUrlTooLong.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();
        var url = "  https://example.com/image.jpg  ";

        // Act
        var result = TbRestaurantImage.Create(id, restaurantId, url, 5);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(restaurantId, result.Value.RestaurantId);
        Assert.Equal("https://example.com/image.jpg", result.Value.ImageUrl); // Verify Trim
        Assert.Equal(5, result.Value.SortOrder);
    }

    // ============================================================
    // UpdateImage Tests
    // ============================================================

    [Fact]
    public void UpdateImage_ShouldReturnError_WhenImageUrlIsInvalid()
    {
        var image = CreateValidImage();

        var result = image.UpdateImage("");

        Assert.False(result.IsSuccess);
        Assert.Equal(RestaurantImageErrors.ImageUrlRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateImage_ShouldReturnSuccess_AndUpdateProperty()
    {
        var image = CreateValidImage();
        var newUrl = "https://example.com/new-image.jpg";

        var result = image.UpdateImage(newUrl);

        Assert.True(result.IsSuccess);
        Assert.Equal(newUrl, image.ImageUrl);
    }

    // ============================================================
    // UpdateSortOrder Tests
    // ============================================================

    [Fact]
    public void UpdateSortOrder_ShouldReturnSuccess_AndUpdateProperty()
    {
        var image = CreateValidImage();
        Assert.Equal(1, image.SortOrder);

        var result = image.UpdateSortOrder(10);

        Assert.True(result.IsSuccess);
        Assert.Equal(10, image.SortOrder);
    }
}
