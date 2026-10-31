using UberSy.Domain.Discounts;
using Xunit;

namespace UberSy.Domain.UnitTests.Discounts;

public class DiscountUsageTests
{
    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenDiscountIdIsEmpty()
    {
        var result = TbDiscountUsage.Create(
            id: Guid.NewGuid(),
            discountId: Guid.Empty,
            customerId: Guid.NewGuid(),
            serviceRequestId: Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal(DiscountErrors.DescountrIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenCustomerIdIsEmpty()
    {
        var result = TbDiscountUsage.Create(
            id: Guid.NewGuid(),
            discountId: Guid.NewGuid(),
            customerId: Guid.Empty,
            serviceRequestId: Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal(DiscountErrors.CustomerIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenServiceRequestIdIsEmpty()
    {
        var result = TbDiscountUsage.Create(
            id: Guid.NewGuid(),
            discountId: Guid.NewGuid(),
            customerId: Guid.NewGuid(),
            serviceRequestId: Guid.Empty);

        Assert.False(result.IsSuccess);
        Assert.Equal(DiscountErrors.ServiceIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_AndSetProperties_WhenDataIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var discountId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var serviceRequestId = Guid.NewGuid();
        var beforeCreation = DateTime.UtcNow;

        // Act
        var result = TbDiscountUsage.Create(
            id: id,
            discountId: discountId,
            customerId: customerId,
            serviceRequestId: serviceRequestId);

        var afterCreation = DateTime.UtcNow;

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(discountId, result.Value.DiscountId);
        Assert.Equal(customerId, result.Value.CustomerId);
        Assert.Equal(serviceRequestId, result.Value.ServiceRequestId);

        // Verify UsedAt is set to UtcNow
        Assert.True(result.Value.UsedAt >= beforeCreation && result.Value.UsedAt <= afterCreation);
    }
}
