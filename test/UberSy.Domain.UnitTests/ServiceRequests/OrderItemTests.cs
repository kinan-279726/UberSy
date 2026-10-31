using UberSy.Domain.ServiceRequests.FoodOrders;
using Xunit;

namespace UberSy.Domain.UnitTests.ServiceRequests.FoodOrders;

public class OrderItemTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbOrderItem CreateValidOrderItem()
    {
        return TbOrderItem.Create(
            id: Guid.NewGuid(),
            foodOrderId: Guid.NewGuid(),
            itemId: Guid.NewGuid(),
            quantity: 2,
            unitPrice: 15.50m,
            note: "Extra spicy"
        ).Value;
    }

    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenItemIdIsEmpty()
    {
        var result = TbOrderItem.Create(
            id: Guid.NewGuid(),
            foodOrderId: Guid.NewGuid(),
            itemId: Guid.Empty,
            quantity: 1,
            unitPrice: 10m,
            note: null);

        Assert.False(result.IsSuccess);
        Assert.Equal(OrderItemErrors.ItemIdRequired.Code, result.TopError.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_ShouldReturnError_WhenQuantityIsZeroOrNegative(int invalidQuantity)
    {
        var result = TbOrderItem.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            quantity: invalidQuantity, unitPrice: 10m, note: null);

        Assert.False(result.IsSuccess);
        Assert.Equal(OrderItemErrors.QuantityZero.Code, result.TopError.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5.5)]
    public void Create_ShouldReturnError_WhenUnitPriceIsZeroOrNegative(decimal invalidPrice)
    {
        var result = TbOrderItem.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            quantity: 1, unitPrice: invalidPrice, note: null);

        Assert.False(result.IsSuccess);
        Assert.Equal(OrderItemErrors.UnitPriceZero.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenNoteIsTooLong()
    {
        var result = TbOrderItem.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            quantity: 1, unitPrice: 10m, note: new string('A', 101));

        Assert.False(result.IsSuccess);
        Assert.Equal(OrderItemErrors.NoteTooLong.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var foodOrderId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var note = "  No onions  ";

        // Act
        var result = TbOrderItem.Create(id, foodOrderId, itemId, 3, 12.99m, note);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal(foodOrderId, result.Value.FoodOrderId);
        Assert.Equal(itemId, result.Value.ItemId);
        Assert.Equal(3, result.Value.Quantity);
        Assert.Equal(12.99m, result.Value.UnitPrice);
        Assert.Equal("No onions", result.Value.Note); // Verify Trim
    }

    // ============================================================
    // Update Tests
    // ============================================================

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void UpdateQuantity_ShouldReturnError_WhenInvalid(int invalidQuantity)
    {
        var orderItem = CreateValidOrderItem();

        var result = orderItem.UpdateQuantity(invalidQuantity);

        Assert.False(result.IsSuccess);
        Assert.Equal(OrderItemErrors.QuantityZero.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateQuantity_ShouldReturnSuccess_AndUpdateProperty()
    {
        var orderItem = CreateValidOrderItem();
        Assert.Equal(2, orderItem.Quantity);

        var result = orderItem.UpdateQuantity(5);

        Assert.True(result.IsSuccess);
        Assert.Equal(5, orderItem.Quantity);
    }

    [Fact]
    public void UpdateNote_ShouldReturnError_WhenTooLong()
    {
        var orderItem = CreateValidOrderItem();

        var result = orderItem.UpdateNote(new string('B', 101));

        Assert.False(result.IsSuccess);
        Assert.Equal(OrderItemErrors.NoteTooLong.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateNote_ShouldReturnSuccess_AndUpdateProperty()
    {
        var orderItem = CreateValidOrderItem();
        var newNote = "  Extra cheese  ";

        var result = orderItem.UpdateNote(newNote);

        Assert.True(result.IsSuccess);
        Assert.Equal("Extra cheese", orderItem.Note); // Verify Trim
    }

    [Fact]
    public void UpdateNote_ShouldAllowSettingToNull()
    {
        var orderItem = CreateValidOrderItem();
        Assert.NotNull(orderItem.Note);

        var result = orderItem.UpdateNote(null);

        Assert.True(result.IsSuccess);
        Assert.Null(orderItem.Note);
    }
}
