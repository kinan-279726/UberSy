using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Helpers;
using UberSy.Application.Features.ServiceRequests.Queries.GetRestaurantStats;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.Queries.GetRestaurantStats;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetRestaurantStatsQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    // Item price 25, tax 5%, so: quantity 2 -> total 52.5, quantity 4 -> total 105
    private async Task<(SeededMenu Menu, SeededOrder DeliveredA, SeededOrder DeliveredB)> SeedDayAsync()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, itemPrice: 25m);

        var deliveredA = await ServiceRequestTestData.SeedOrderAsync(
            Context, menu, status: ServiceRequestStatus.Delivered, quantity: 2,
            deliveryFee: 7500m, driverEarning: 6000m, commission: 5m);
        var deliveredB = await ServiceRequestTestData.SeedOrderAsync(
            Context, menu, status: ServiceRequestStatus.Delivered, quantity: 4,
            deliveryFee: 7500m, driverEarning: 6000m, commission: 10m);

        await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Cancelled);
        await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Pending);
        await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Preparing);
        await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.OutForDelivery);

        return (menu, deliveredA, deliveredB);
    }

    [Fact]
    public async Task Handle_ShouldCountOrdersByStatus_ForToday()
    {
        var (menu, _, _) = await SeedDayAsync();
        AsOwner1();

        var result = await Mediator.Send(new GetRestaurantStatsQuery(menu.Restaurant.Id, null, null), CancellationToken.None);

        AssertSuccess(result);
        var stats = result.Value;
        Assert.Equal(DamascusTime.Today(), stats.From);
        Assert.Equal(DamascusTime.Today(), stats.To);
        Assert.Equal(6, stats.TotalOrders);
        Assert.Equal(2, stats.DeliveredOrders);
        Assert.Equal(1, stats.CancelledOrders);
        Assert.Equal(3, stats.InProgressOrders);   // pending + preparing + out for delivery
    }

    [Fact]
    public async Task Handle_ShouldCalculateMoneyFromDeliveredOrdersOnly()
    {
        var (menu, _, _) = await SeedDayAsync();
        AsOwner1();

        var result = await Mediator.Send(new GetRestaurantStatsQuery(menu.Restaurant.Id, null, null), CancellationToken.None);

        AssertSuccess(result);
        var stats = result.Value;
        Assert.Equal(157.5m, stats.GrossSales);          // 52.5 + 105
        Assert.Equal(15m, stats.PlatformCommission);     // 5 + 10
        Assert.Equal(142.5m, stats.NetEarnings);
        Assert.Equal(78.75m, stats.AverageOrderValue);   // 157.5 / 2
    }

    [Fact]
    public async Task Handle_ShouldBuildDailyBreakdownAndTopItems()
    {
        var (menu, _, _) = await SeedDayAsync();
        AsOwner1();

        var result = await Mediator.Send(new GetRestaurantStatsQuery(menu.Restaurant.Id, null, null), CancellationToken.None);

        AssertSuccess(result);
        var day = Assert.Single(result.Value.Daily);
        Assert.Equal(DamascusTime.Today(), day.Date);
        Assert.Equal(2, day.DeliveredOrders);
        Assert.Equal(157.5m, day.GrossSales);
        Assert.Equal(142.5m, day.NetEarnings);

        var top = Assert.Single(result.Value.TopItems);
        Assert.Equal(menu.Item.Id, top.ItemId);
        Assert.Equal(menu.Item.NameEn, top.NameEn);
        Assert.Equal(6, top.QuantitySold);               // delivered orders only: 2 + 4
    }

    [Fact]
    public async Task Handle_ShouldRankTopItemsByQuantity_AndKeepFive()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, itemPrice: 25m);
        AsOwner1();

        // 6 items, the one sold least must be dropped
        var items = new List<(Guid Id, int Quantity)> { (menu.Item.Id, 1) };
        for (var i = 2; i <= 6; i++)
        {
            var extra = await ServiceRequestTestData.SeedExtraItemAsync(Context, menu, price: 25m, sortOrder: (byte)i);
            items.Add((extra.Id, i));
        }

        foreach (var (id, quantity) in items)
        {
            var itemMenu = menu with { Item = Context.TbItems.Single(x => x.Id == id) };
            await ServiceRequestTestData.SeedOrderAsync(
                Context, itemMenu, status: ServiceRequestStatus.Delivered, quantity: quantity + 1,
                deliveryFee: 7500m, driverEarning: 6000m, commission: 1m);
        }

        var result = await Mediator.Send(new GetRestaurantStatsQuery(menu.Restaurant.Id, null, null), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(5, result.Value.TopItems.Count);
        Assert.DoesNotContain(result.Value.TopItems, t => t.ItemId == menu.Item.Id);
        Assert.Equal(
            result.Value.TopItems.Select(t => t.QuantitySold).OrderByDescending(q => q).ToArray(),
            result.Value.TopItems.Select(t => t.QuantitySold).ToArray());
    }

    [Fact]
    public async Task Handle_ShouldHonourTheDateRange()
    {
        var (menu, deliveredA, _) = await SeedDayAsync();
        await ServiceRequestTestData.BackdateAsync(Context, deliveredA.Id, DateTimeOffset.UtcNow.AddDays(-10));
        AsOwner1();

        var today = await Mediator.Send(new GetRestaurantStatsQuery(menu.Restaurant.Id, null, null), CancellationToken.None);

        var from = DamascusTime.Today().AddDays(-12);
        var to = DamascusTime.Today().AddDays(-8);
        var past = await Mediator.Send(new GetRestaurantStatsQuery(menu.Restaurant.Id, from, to), CancellationToken.None);

        AssertSuccess(today);
        Assert.Equal(1, today.Value.DeliveredOrders);
        Assert.Equal(105m, today.Value.GrossSales);

        AssertSuccess(past);
        Assert.Equal(from, past.Value.From);
        Assert.Equal(to, past.Value.To);
        Assert.Equal(1, past.Value.TotalOrders);
        Assert.Equal(1, past.Value.DeliveredOrders);
        Assert.Equal(52.5m, past.Value.GrossSales);
    }

    [Fact]
    public async Task Handle_ShouldReturnZeros_WhenNoOrdersInRange()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        AsOwner1();

        var result = await Mediator.Send(new GetRestaurantStatsQuery(menu.Restaurant.Id, null, null), CancellationToken.None);

        AssertSuccess(result);
        var stats = result.Value;
        Assert.Equal(0, stats.TotalOrders);
        Assert.Equal(0, stats.DeliveredOrders);
        Assert.Equal(0m, stats.GrossSales);
        Assert.Equal(0m, stats.AverageOrderValue);
        Assert.Empty(stats.Daily);
        Assert.Empty(stats.TopItems);
    }

    [Fact]
    public async Task Handle_ShouldNotCountOtherRestaurantsOrders()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, TestUsers.Owner1.Id);
        var otherMenu = await ServiceRequestTestData.SeedMenuAsync(Context, TestUsers.Owner2.Id);
        await ServiceRequestTestData.SeedOrderAsync(
            Context, otherMenu, status: ServiceRequestStatus.Delivered, deliveryFee: 7500m, driverEarning: 6000m, commission: 5m);
        AsOwner1();

        var result = await Mediator.Send(new GetRestaurantStatsQuery(menu.Restaurant.Id, null, null), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(0, result.Value.TotalOrders);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenAnotherOwner()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, TestUsers.Owner1.Id);
        AsOwner2();

        var result = await Mediator.Send(new GetRestaurantStatsQuery(menu.Restaurant.Id, null, null), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);

        var result = await Mediator.Send(new GetRestaurantStatsQuery(menu.Restaurant.Id, null, null), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenRangeIsInverted()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        AsOwner1();
        var today = DamascusTime.Today();

        var result = await Mediator.Send(
            new GetRestaurantStatsQuery(menu.Restaurant.Id, today, today.AddDays(-1)), CancellationToken.None);

        Assert.True(result.IsError);
    }
}
