using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.ServiceRequests.Commands.PlaceFoodOrder;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Application.SubcutaneousTests.Common.Fakes;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.PlaceFoodOrder;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class PlaceFoodOrderCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    // Restaurant defaults (RestaurantFactory): tax 5%, commission 10%, min order 20, max order 500.
    // Routing is faked at 3 km and pricing falls back to the configured defaults (2500 / km customer rate).

    private static PlaceFoodOrderCommand Command(SeededMenu menu, Guid addressId, int quantity = 2, params Guid[] addOnIds)
        => new(menu.Restaurant.Id, addressId, [new PlaceFoodOrderItem(menu.Item.Id, quantity, addOnIds.ToList())]);

    [Fact]
    public async Task Handle_ShouldPlaceOrderAndReturnSummary()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, itemPrice: 25m);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var result = await Mediator.Send(Command(menu, addressId, quantity: 2), CancellationToken.None);

        AssertSuccess(result);
        var summary = result.Value;
        Assert.Equal(menu.Restaurant.Id, summary.RestaurantId);
        Assert.Equal((byte)ServiceRequestStatus.Pending, summary.Status);
        Assert.Equal(50m, summary.ItemsTotal);
        Assert.Equal(0m, summary.DiscountAmount);
        Assert.Equal(2.5m, summary.TaxAmount);
        Assert.Equal(52.5m, summary.TotalAmount);
        Assert.Equal(FakeRoutingService.DefaultDistanceKm, summary.DistanceKm);
        Assert.Equal(FakeRoutingService.DefaultDistanceKm * TestSettings.CustomerRatePerKm, summary.EstimatedDeliveryFee);
        Assert.Equal(52.5m + summary.EstimatedDeliveryFee, summary.EstimatedGrandTotal);

        var expectedDeadline = DateTimeOffset.UtcNow.AddMinutes(TestSettings.RestaurantResponseTimeoutMinutes);
        Assert.InRange(summary.ResponseDeadlineUtc, expectedDeadline.AddMinutes(-1), expectedDeadline.AddMinutes(1));
    }

    [Fact]
    public async Task Handle_ShouldPersistServiceRequestFoodOrderAndLines()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, itemPrice: 25m);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var result = await Mediator.Send(Command(menu, addressId, quantity: 2), CancellationToken.None);

        AssertSuccess(result);

        var order = await Context.TbFoodOrders.AsNoTracking().Include(o => o.IdNavigation)
            .SingleAsync(o => o.Id == result.Value.ServiceRequestId);
        Assert.Equal(TestUsers.Customer.Id, order.IdNavigation.CustomerId);
        Assert.Equal(ServiceType.Food, order.IdNavigation.ServiceType);
        Assert.Equal(ServiceRequestStatus.Pending, order.IdNavigation.Status);
        Assert.Equal(52.5m, order.IdNavigation.TotalAmount);
        Assert.Equal(menu.Restaurant.Id, order.RestaurantId);
        Assert.Equal(addressId, order.AddressId);
        Assert.Null(order.DriverId);

        // The line snapshots the item price at order time
        var line = await Context.TbOrderItems.AsNoTracking().SingleAsync(i => i.FoodOrderId == order.Id);
        Assert.Equal(menu.Item.Id, line.ItemId);
        Assert.Equal(2, line.Quantity);
        Assert.Equal(25m, line.UnitPrice);
    }

    [Fact]
    public async Task Handle_ShouldIncludeAddOnsInTotalsAndSnapshotTheirPrice()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, itemPrice: 25m, withAddOn: true, addOnPrice: 5m);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var result = await Mediator.Send(Command(menu, addressId, 2, menu.AddOn!.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(60m, result.Value.ItemsTotal);   // (25 + 5) * 2
        Assert.Equal(3m, result.Value.TaxAmount);     // 5% of 60
        Assert.Equal(63m, result.Value.TotalAmount);

        var addOnRow = await Context.TbOrderItemAddOns.AsNoTracking().SingleAsync();
        Assert.Equal(menu.AddOn.Id, addOnRow.AddOnId);
        Assert.Equal(5m, addOnRow.Price);
    }

    [Fact]
    public async Task Handle_ShouldCountDuplicatedAddOnIdsOnce()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, itemPrice: 25m, withAddOn: true, addOnPrice: 5m);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var result = await Mediator.Send(Command(menu, addressId, 1, menu.AddOn!.Id, menu.AddOn.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(30m, result.Value.ItemsTotal);
        Assert.Equal(1, await Context.TbOrderItemAddOns.AsNoTracking().CountAsync());
    }

    [Fact]
    public async Task Handle_ShouldSupportSeveralDifferentItems()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, itemPrice: 25m);
        var second = await ServiceRequestTestData.SeedExtraItemAsync(Context, menu, price: 40m);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var command = new PlaceFoodOrderCommand(menu.Restaurant.Id, addressId,
        [
            new PlaceFoodOrderItem(menu.Item.Id, 1, []),
            new PlaceFoodOrderItem(second.Id, 2, []),
        ]);

        var result = await Mediator.Send(command, CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(105m, result.Value.ItemsTotal);  // 25 + 40 * 2
        Assert.Equal(2, await Context.TbOrderItems.AsNoTracking().CountAsync());
    }

    [Fact]
    public async Task Handle_ShouldNotifyRestaurantOfNewOrder()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var result = await Mediator.Send(Command(menu, addressId), CancellationToken.None);

        AssertSuccess(result);
        var notification = Assert.Single(Notifier.Of(nameof(IRestaurantNotifier.NotifyNewOrderAsync)));
        Assert.Equal(menu.Restaurant.Id, notification.RecipientId);
        Assert.Equal(result.Value.ServiceRequestId, notification.ServiceRequestId);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);

        var result = await Mediator.Send(Command(menu, addressId), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
        Assert.False(await Context.TbServiceRequests.AsNoTracking().AnyAsync());
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenRestaurantMissing()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var command = new PlaceFoodOrderCommand(Guid.NewGuid(), addressId, [new PlaceFoodOrderItem(menu.Item.Id, 1, [])]);

        var result = await Mediator.Send(command, CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotApproved_WhenRestaurantIsPending()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, approved: false, open: false);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var result = await Mediator.Send(Command(menu, addressId), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotApproved);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantClosed_WhenRestaurantIsClosed()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, approved: true, open: false);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var result = await Mediator.Send(Command(menu, addressId), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantClosed);
    }

    [Fact]
    public async Task Handle_ShouldReturnAddressNotFound_WhenAddressMissing()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        AsCustomer();

        var result = await Mediator.Send(Command(menu, Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.AddressNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnAddressNotFound_WhenAddressBelongsToAnotherUser()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var othersAddress = await ServiceRequestTestData.SeedAddressAsync(Context, TestUsers.Customer2.Id);
        AsCustomer();

        var result = await Mediator.Send(Command(menu, othersAddress), CancellationToken.None);

        AssertError(result, ApplicationErrors.AddressNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnItemNotFound_WhenItemMissing()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var command = new PlaceFoodOrderCommand(menu.Restaurant.Id, addressId, [new PlaceFoodOrderItem(Guid.NewGuid(), 1, [])]);

        var result = await Mediator.Send(command, CancellationToken.None);

        AssertError(result, ApplicationErrors.ItemNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnItemNotInRestaurant_WhenItemBelongsToAnotherRestaurant()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, TestUsers.Owner1.Id);
        var otherMenu = await ServiceRequestTestData.SeedMenuAsync(Context, TestUsers.Owner2.Id);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var command = new PlaceFoodOrderCommand(menu.Restaurant.Id, addressId, [new PlaceFoodOrderItem(otherMenu.Item.Id, 1, [])]);

        var result = await Mediator.Send(command, CancellationToken.None);

        AssertError(result, ApplicationErrors.ItemNotInRestaurant);
    }

    [Fact]
    public async Task Handle_ShouldReject_WhenItemIsInactive()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        menu.Item.Deactivate();
        await Context.SaveChangesAsync(CancellationToken.None);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var result = await Mediator.Send(Command(menu, addressId), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.False(await Context.TbServiceRequests.AsNoTracking().AnyAsync());
    }

    [Fact]
    public async Task Handle_ShouldReturnAddOnNotAvailable_WhenAddOnIsNotAssignedToItem()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var unassigned = await RestaurantTestData.SeedAddOnAsync(Context, menu.Restaurant.Id);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var result = await Mediator.Send(Command(menu, addressId, 1, unassigned.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.AddOnNotAvailableForItem);
    }

    [Fact]
    public async Task Handle_ShouldReject_WhenAddOnIsInactive()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, withAddOn: true);
        menu.AddOn!.Deactivate();
        await Context.SaveChangesAsync(CancellationToken.None);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var result = await Mediator.Send(Command(menu, addressId, 1, menu.AddOn.Id), CancellationToken.None);

        Assert.True(result.IsError);
    }

    [Fact]
    public async Task Handle_ShouldReturnBelowMinimum_WhenItemsTotalIsUnderRestaurantMinimum()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, itemPrice: 5m);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var result = await Mediator.Send(Command(menu, addressId, quantity: 1), CancellationToken.None);

        AssertError(result, ApplicationErrors.BelowMinimumOrderAmount);
    }

    [Fact]
    public async Task Handle_ShouldReturnAboveMaximum_WhenItemsTotalExceedsRestaurantMaximum()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, itemPrice: 25m);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var result = await Mediator.Send(Command(menu, addressId, quantity: 30), CancellationToken.None); // 750 > 500

        AssertError(result, ApplicationErrors.AboveMaximumOrderAmount);
    }

    [Fact]
    public async Task Handle_ShouldReturnDistanceTooLong_WhenAddressIsBeyondMaxDeliveryDistance()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        Routing.DistanceKm = TestSettings.MaxDistanceKm + 5m;
        AsCustomer();

        var result = await Mediator.Send(Command(menu, addressId), CancellationToken.None);

        AssertError(result, ApplicationErrors.DeliveryDistanceTooLong);
        Assert.False(await Context.TbServiceRequests.AsNoTracking().AnyAsync());
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenItemsEmpty()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var result = await Mediator.Send(
            new PlaceFoodOrderCommand(menu.Restaurant.Id, addressId, []), CancellationToken.None);

        AssertErrorCode(result, "Items");
    }

    [Fact]
    public async Task Handle_ShouldNotPersistAnything_WhenValidationFails()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        await Mediator.Send(Command(menu, addressId, quantity: 0), CancellationToken.None);

        Assert.False(await Context.TbServiceRequests.AsNoTracking().AnyAsync());
        Assert.False(await Context.TbOrderItems.AsNoTracking().AnyAsync());
        Assert.Empty(Notifier.Of(nameof(IRestaurantNotifier.NotifyNewOrderAsync)));
    }
}
