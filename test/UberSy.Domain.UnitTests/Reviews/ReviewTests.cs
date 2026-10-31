using UberSy.Domain.Reviews;
using Xunit;

namespace UberSy.Domain.UnitTests.Reviews;

public class ReviewTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbReview CreateValidRestaurantReview()
    {
        return TbReview.Create(
            id: Guid.NewGuid(),
            customerId: Guid.NewGuid(),
            serviceRequestId: Guid.NewGuid(),
            restaurantId: Guid.NewGuid(), // Target is Restaurant
            driverId: null,
            rating: 4,
            note: "Great food!"
        ).Value;
    }

    // ============================================================
    // Create Tests - IDs
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenCustomerIdIsEmpty()
    {
        var result = TbReview.Create(
            Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), Guid.NewGuid(), null, 5, null);

        Assert.False(result.IsSuccess);
        Assert.Equal(ReviewErrors.CustomerIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenServiceRequestIdIsEmpty()
    {
        var result = TbReview.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), null, 5, null);

        Assert.False(result.IsSuccess);
        Assert.Equal(ReviewErrors.ServiceRequestIdRequired.Code, result.TopError.Code);
    }

    // ============================================================
    // Create Tests - Target Validation
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenBothTargetsAreProvided()
    {
        var result = TbReview.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), // Restaurant
            Guid.NewGuid(), // Driver
            5, null);

        Assert.False(result.IsSuccess);
        Assert.Equal(ReviewErrors.ExactlyOneTargetRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenNoTargetIsProvided()
    {
        var result = TbReview.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            null, // No Restaurant
            null, // No Driver
            5, null);

        Assert.False(result.IsSuccess);
        Assert.Equal(ReviewErrors.ExactlyOneTargetRequired.Code, result.TopError.Code);
    }

    // ============================================================
    // Create Tests - Rating & Note
    // ============================================================

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Create_ShouldReturnError_WhenRatingIsOutOfRange(byte invalidRating)
    {
        var result = TbReview.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, invalidRating, null);

        Assert.False(result.IsSuccess);
        Assert.Equal(ReviewErrors.RatingOutOfRange.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenNoteIsTooLong()
    {
        var result = TbReview.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), null, 5, new string('A', 1001));

        Assert.False(result.IsSuccess);
        Assert.Equal(ReviewErrors.NoteTooLong.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenTargetIsRestaurant()
    {
        // Arrange
        var id = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();

        // Act
        var result = TbReview.Create(
            id, Guid.NewGuid(), Guid.NewGuid(), restaurantId, null, 4, "  Nice place  ");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(restaurantId, result.Value.RestaurantId);
        Assert.Null(result.Value.DriverId);
        Assert.Equal(4, result.Value.Rating);
        Assert.Equal("Nice place", result.Value.Note); // Verify Trim
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenTargetIsDriver()
    {
        // Arrange
        var id = Guid.NewGuid();
        var driverId = Guid.NewGuid();

        // Act
        var result = TbReview.Create(
            id, Guid.NewGuid(), Guid.NewGuid(), null, driverId, 5, "Fast delivery");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
        Assert.Null(result.Value.RestaurantId);
        Assert.Equal(driverId, result.Value.DriverId);
        Assert.Equal(5, result.Value.Rating);
    }
}
