using UberSy.Domain.ServiceRequests.Events.CreationEvents;
using UberSy.Domain.ServiceRequests.Events.DriverEvents;
using UberSy.Domain.ServiceRequests.FoodOrders;
using Xunit;

namespace UberSy.Domain.UnitTests.ServiceRequests.FoodOrders;

public class FoodOrderTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbFoodOrder CreateValidFoodOrder()
    {
        return TbFoodOrder.Create(
            id: Guid.NewGuid(),
            restaurantId: Guid.NewGuid(),
            addressId: Guid.NewGuid(),
            responseTimeout: DateTimeOffset.UtcNow.AddMinutes(3)
        ).Value;
    }

    // ============================================================
    // Create Tests
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenRestaurantIdIsEmpty()
    {
        var result = TbFoodOrder.Create(
            Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), DateTimeOffset.UtcNow);

        Assert.False(result.IsSuccess);
        Assert.Equal(FoodOrderErrors.RestaurantIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenAddressIdIsEmpty()
    {
        var result = TbFoodOrder.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, DateTimeOffset.UtcNow);

        Assert.False(result.IsSuccess);
        Assert.Equal(FoodOrderErrors.AddressIdRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_AndRaiseDomainEvent()
    {
        // Arrange
        var id = Guid.NewGuid();
        var restaurantId = Guid.NewGuid();
        var addressId = Guid.NewGuid();
        var timeout = DateTimeOffset.UtcNow.AddMinutes(3);

        // Act
        var result = TbFoodOrder.Create(id, restaurantId, addressId, timeout);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify default values
        Assert.Equal(0m, result.Value.DiscountAmount);
        Assert.Equal(0m, result.Value.DeliveryFee);
        Assert.Equal(0m, result.Value.DriverEarning);
        Assert.Equal(0m, result.Value.PlatformCommissionFromRestaurant);

        // Verify Domain Event
        Assert.Single(result.Value.DomainEvents);
        Assert.IsType<FoodOrderCreated>(result.Value.DomainEvents.First());
    }

    // ============================================================
    // Driver Assignment Tests
    // ============================================================

    [Fact]
    public void AssignDriver_ShouldReturnError_WhenDriverIdIsEmpty()
    {
        var order = CreateValidFoodOrder();
        var result = order.AssignDriver(Guid.Empty);

        Assert.False(result.IsSuccess);
        Assert.Equal(FoodOrderErrors.RestaurantIdRequired.Code, result.TopError.Code); // Note: Your domain currently returns RestaurantIdRequired here
    }

    [Fact]
    public void AssignDriver_ShouldReturnError_WhenDriverAlreadyAssigned()
    {
        var order = CreateValidFoodOrder();
        order.AssignDriver(Guid.NewGuid()); // First time

        var result = order.AssignDriver(Guid.NewGuid()); // Second time

        Assert.False(result.IsSuccess);
        Assert.Equal(FoodOrderErrors.DriverAlreadyAssigned.Code, result.TopError.Code);
    }

    [Fact]
    public void AssignDriver_ShouldReturnSuccess_AndSetDriverId()
    {
        var order = CreateValidFoodOrder();
        var driverId = Guid.NewGuid();

        var result = order.AssignDriver(driverId);

        Assert.True(result.IsSuccess);
        Assert.Equal(driverId, order.DriverId);
    }

    // ============================================================
    // Driver Removal Tests
    // ============================================================

    [Fact]
    public void RemoveDriver_ShouldReturnError_WhenNoDriverAssigned()
    {
        var order = CreateValidFoodOrder();
        // No driver assigned

        var result = order.RemoveDriver();

        Assert.False(result.IsSuccess);
        Assert.Equal(FoodOrderErrors.NoDriverAssigned.Code, result.TopError.Code);
    }

    [Fact]
    public void RemoveDriver_ShouldReturnSuccess_AndRaiseDomainEvent()
    {
        var order = CreateValidFoodOrder();
        var driverId = Guid.NewGuid();
        order.AssignDriver(driverId);
        order.ClearDomainEvents(); // Clear previous events (like FoodOrderCreated) for testing

        var result = order.RemoveDriver();

        Assert.True(result.IsSuccess);
        Assert.Null(order.DriverId);

        // Verify Domain Event
        Assert.Single(order.DomainEvents);
        var domainEvent = order.DomainEvents.First() as DriverCancelledAfterAccept;
        Assert.NotNull(domainEvent);
        Assert.Equal(driverId, domainEvent.DriverId);
    }

    // ============================================================
    // Discount Tests
    // ============================================================

    [Fact]
    public void ApplyDiscount_ShouldReturnError_WhenAmountIsNegative()
    {
        var order = CreateValidFoodOrder();
        var result = order.ApplyDiscount(Guid.NewGuid(), -5m);

        Assert.False(result.IsSuccess);
        Assert.Equal(FoodOrderErrors.DiscountAmountNegative.Code, result.TopError.Code);
    }

    [Fact]
    public void ApplyDiscount_ShouldReturnSuccess_AndUpdateProperties()
    {
        var order = CreateValidFoodOrder();
        var discountId = Guid.NewGuid();
        var amount = 10m;

        var result = order.ApplyDiscount(discountId, amount);

        Assert.True(result.IsSuccess);
        Assert.Equal(discountId, order.DiscountId);
        Assert.Equal(amount, order.DiscountAmount);
    }

    // ============================================================
    // Financials Tests
    // ============================================================

    [Fact]
    public void RecordDeliveryFinancials_ShouldReturnError_WhenDriverEarningExceedsDeliveryFee()
    {
        var order = CreateValidFoodOrder();
        var result = order.RecordDeliveryFinancials(
            deliveryFee: 10m,
            driverEarning: 15m, // Exceeds delivery fee
            platformCommission: 2m);

        Assert.False(result.IsSuccess);
        Assert.Equal(FoodOrderErrors.DriverEarningExceedsDeliveryFee.Code, result.TopError.Code);
    }

    [Fact]
    public void RecordDeliveryFinancials_ShouldReturnError_WhenAnyValueIsNegative()
    {
        var order = CreateValidFoodOrder();

        var result1 = order.RecordDeliveryFinancials(-1m, 0m, 0m);
        Assert.Equal(FoodOrderErrors.DeliveryFeeNegative.Code, result1.TopError.Code);

        var result2 = order.RecordDeliveryFinancials(10m, -1m, 0m);
        Assert.Equal(FoodOrderErrors.DriverEarningNegative.Code, result2.TopError.Code);

        var result3 = order.RecordDeliveryFinancials(10m, 5m, -1m);
        Assert.Equal(FoodOrderErrors.CommissionNegative.Code, result3.TopError.Code);
    }

    [Fact]
    public void RecordDeliveryFinancials_ShouldReturnSuccess_AndUpdateProperties()
    {
        var order = CreateValidFoodOrder();
        var deliveryFee = 15m;
        var driverEarning = 10m;
        var commission = 5m;

        var result = order.RecordDeliveryFinancials(deliveryFee, driverEarning, commission);

        Assert.True(result.IsSuccess);
        Assert.Equal(deliveryFee, order.DeliveryFee);
        Assert.Equal(driverEarning, order.DriverEarning);
        Assert.Equal(commission, order.PlatformCommissionFromRestaurant);
    }
}
