using UberSy.Domain.ServiceRequests.FoodOrders;
using Xunit;

namespace UberSy.Domain.UnitTests.ServiceRequests.FoodOrders;

public class OrderItemAddOnTests
{
    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenAddOnIdIsEmpty()
    {
        var result = TbOrderItemAddOn.Create(
            id: Guid.NewGuid(),
            orderItemId: Guid.NewGuid(),
            addOnId: Guid.Empty,
            price: 5.00m);

        Assert.False(result.IsSuccess);
        Assert.Equal(OrderItemErrors.ItemIdRequired.Code, result.TopError.Code); // Note: Your domain uses ItemIdRequired here
    }

    [Fact]
    public void Create_ShouldReturnError_WhenPriceIsNegative()
    {
        var result = TbOrderItemAddOn.Create(
            id: Guid.NewGuid(),
            orderItemId: Guid.NewGuid(),
            addOnId: Guid.NewGuid(),
            price: -1.00m);

        Assert.False(result.IsSuccess);
        Assert.Equal(OrderItemErrors.AddOnPriceNegative.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid_WithPositivePrice()
    {
        // Arrange
        var id = Guid.NewGuid();
        var orderItemId = Guid.NewGuid();
        var addOnId = Guid.NewGuid();
        var price = 3.50m;

        // Act
        var result = TbOrderItemAddOn.Create(id, orderItemId, addOnId, price);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(orderItemId, result.Value.OrderItemId);
        Assert.Equal(addOnId, result.Value.AddOnId);
        Assert.Equal(price, result.Value.Price);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenPriceIsZero()
    {
        // Arrange
        var id = Guid.NewGuid();
        var orderItemId = Guid.NewGuid();
        var addOnId = Guid.NewGuid();
        var price = 0m; // Free add-on

        // Act
        var result = TbOrderItemAddOn.Create(id, orderItemId, addOnId, price);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(0m, result.Value.Price);
    }
}
