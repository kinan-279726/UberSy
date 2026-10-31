using UberSy.Domain.Discounts;
using UberSy.Domain.Discounts.Enums;
using Xunit;

namespace UberSy.Domain.UnitTests.Discounts;

public class DiscountTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbDiscount CreateValidDiscount()
    {
        return TbDiscount.Create(
            id: Guid.NewGuid(),
            restaurantId: Guid.NewGuid(),
            code: "SAVE10",
            type: DiscountType.Percentage,
            value: 10m,
            scope: DiscountScope.Order,
            minOrderAmount: 50m,
            maxUsageCount: 100,
            maxUsagePerCustomer: 1,
            startDate: DateTime.UtcNow,
            endDate: DateTime.UtcNow.AddDays(7)
        ).Value;
    }

    // ============================================================
    // Create Tests - Code & Type
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenCodeIsTooLong()
    {
        var result = TbDiscount.Create(
            Guid.NewGuid(), Guid.NewGuid(), new string('A', 21),
            DiscountType.FixedAmount, 10m, DiscountScope.Order,
            null, null, null, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        Assert.False(result.IsSuccess);
        Assert.Equal(DiscountErrors.CodeTooLong.Code, result.TopError.Code);
    }

    // ============================================================
    // Create Tests - Value Validation
    // ============================================================

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Create_ShouldReturnError_WhenValueIsZeroOrNegative(decimal invalidValue)
    {
        var result = TbDiscount.Create(
            Guid.NewGuid(), Guid.NewGuid(), "TEST",
            DiscountType.FixedAmount, invalidValue, DiscountScope.Order,
            null, null, null, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        Assert.False(result.IsSuccess);
        Assert.Equal(DiscountErrors.ValueZero.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenPercentageExceeds100()
    {
        var result = TbDiscount.Create(
            Guid.NewGuid(), Guid.NewGuid(), "TEST",
            DiscountType.Percentage, 101m, DiscountScope.Order,
            null, null, null, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        Assert.False(result.IsSuccess);
        Assert.Equal(DiscountErrors.PercentageExceeds100.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldAllowFixedAmountGreaterThan100()
    {
        // A fixed amount discount (e.g., 150 EGP off) is valid, only percentages are capped at 100.
        var result = TbDiscount.Create(
            Guid.NewGuid(), Guid.NewGuid(), "TEST",
            DiscountType.FixedAmount, 150m, DiscountScope.Order,
            null, null, null, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        Assert.True(result.IsSuccess);
    }

    // ============================================================
    // Create Tests - Limits & Dates
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenMinOrderAmountIsNegative()
    {
        var result = TbDiscount.Create(
            Guid.NewGuid(), Guid.NewGuid(), "TEST",
            DiscountType.FixedAmount, 10m, DiscountScope.Order,
            -5m, null, null, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        Assert.False(result.IsSuccess);
        Assert.Equal(DiscountErrors.MinOrderAmountNegative.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenMaxUsageCountIsZero()
    {
        var result = TbDiscount.Create(
            Guid.NewGuid(), Guid.NewGuid(), "TEST",
            DiscountType.FixedAmount, 10m, DiscountScope.Order,
            null, 0, null, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        Assert.False(result.IsSuccess);
        Assert.Equal(DiscountErrors.MaxUsageCountZero.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenEndDateIsBeforeStartDate()
    {
        var result = TbDiscount.Create(
            Guid.NewGuid(), Guid.NewGuid(), "TEST",
            DiscountType.FixedAmount, 10m, DiscountScope.Order,
            null, null, null, DateTime.UtcNow, DateTime.UtcNow.AddDays(-1));

        Assert.False(result.IsSuccess);
        Assert.Equal(DiscountErrors.EndDateBeforeStartDate.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        var result = TbDiscount.Create(
            Guid.NewGuid(), Guid.NewGuid(), "  SAVE10  ",
            DiscountType.Percentage, 10m, DiscountScope.Order,
            50m, 100, 1, DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

        Assert.True(result.IsSuccess);
        Assert.Equal("SAVE10", result.Value.Code); // Verify Trim()
        Assert.True(result.Value.IsActive); // Default should be active
    }

    // ============================================================
    // Update Tests
    // ============================================================

    [Fact]
    public void UpdateValue_ShouldReturnError_WhenPercentageExceeds100()
    {
        var discount = CreateValidDiscount();

        var result = discount.UpdateValue(101m);

        Assert.False(result.IsSuccess);
        Assert.Equal(DiscountErrors.PercentageExceeds100.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateValue_ShouldReturnSuccess_AndUpdateProperty()
    {
        var discount = CreateValidDiscount();

        var result = discount.UpdateValue(15m);

        Assert.True(result.IsSuccess);
        Assert.Equal(15m, discount.Value);
    }

    [Fact]
    public void UpdateSchedule_ShouldReturnError_WhenEndDateBeforeStartDate()
    {
        var discount = CreateValidDiscount();

        var result = discount.UpdateSchedule(DateTime.UtcNow, DateTime.UtcNow.AddDays(-1));

        Assert.False(result.IsSuccess);
        Assert.Equal(DiscountErrors.EndDateBeforeStartDate.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateSchedule_ShouldReturnSuccess_AndUpdateProperties()
    {
        var discount = CreateValidDiscount();
        var newStart = DateTime.UtcNow.AddDays(2);
        var newEnd = DateTime.UtcNow.AddDays(10);

        var result = discount.UpdateSchedule(newStart, newEnd);

        Assert.True(result.IsSuccess);
        Assert.Equal(newStart, discount.StartDate);
        Assert.Equal(newEnd, discount.EndDate);
    }

    // ============================================================
    // State Transition Tests
    // ============================================================

    [Fact]
    public void Deactivate_ShouldReturnSuccess_AndSetIsActiveToFalse()
    {
        var discount = CreateValidDiscount();
        Assert.True(discount.IsActive);

        var result = discount.Deactivate();

        Assert.True(result.IsSuccess);
        Assert.False(discount.IsActive);
    }

    [Fact]
    public void Deactivate_ShouldReturnError_WhenAlreadyInactive()
    {
        var discount = CreateValidDiscount();
        discount.Deactivate(); // Set to false

        var result = discount.Deactivate(); // Try again

        Assert.False(result.IsSuccess);
        Assert.Equal(DiscountErrors.DiscountAlreadyInactive.Code, result.TopError.Code);
    }

    [Fact]
    public void Activate_ShouldReturnSuccess_AndSetIsActiveToTrue()
    {
        var discount = CreateValidDiscount();
        discount.Deactivate(); // Set to false first
        Assert.False(discount.IsActive);

        var result = discount.Activate();

        Assert.True(result.IsSuccess);
        Assert.True(discount.IsActive);
    }

    [Fact]
    public void Activate_ShouldReturnError_WhenAlreadyActive()
    {
        var discount = CreateValidDiscount();
        Assert.True(discount.IsActive);

        var result = discount.Activate(); // Try again

        Assert.False(result.IsSuccess);
        Assert.Equal(DiscountErrors.DiscountAlreadyActive.Code, result.TopError.Code);
    }
}
