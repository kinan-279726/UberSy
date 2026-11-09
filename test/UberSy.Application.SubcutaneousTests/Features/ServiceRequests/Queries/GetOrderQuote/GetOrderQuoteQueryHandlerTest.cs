using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Features.ServiceRequests.Commands.PlaceFoodOrder;
using UberSy.Application.Features.ServiceRequests.Queries.GetOrderQuote;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Application.SubcutaneousTests.Common.Fakes;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.Queries.GetOrderQuote;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetOrderQuoteQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldReturnQuoteWithLinesTaxAndDelivery()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, itemPrice: 25m, withAddOn: true, addOnPrice: 5m);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var query = new GetOrderQuoteQuery(menu.Restaurant.Id, addressId,
            [new PlaceFoodOrderItem(menu.Item.Id, 2, [menu.AddOn!.Id])]);

        var result = await Mediator.Send(query, CancellationToken.None);

        AssertSuccess(result);
        var quote = result.Value;
        Assert.Equal(menu.Restaurant.Id, quote.RestaurantId);
        Assert.Equal(menu.Restaurant.NameAr, quote.RestaurantNameAr);

        var line = Assert.Single(quote.Lines);
        Assert.Equal(menu.Item.Id, line.ItemId);
        Assert.Equal(2, line.Quantity);
        Assert.Equal(25m, line.UnitPrice);
        Assert.Equal(60m, line.LineTotal);                       // (25 + 5) * 2
        Assert.Equal(new[] { menu.AddOn.NameAr }, line.AddOnNamesAr.ToArray());

        Assert.Equal(60m, quote.ItemsTotal);
        Assert.Equal(0m, quote.DiscountAmount);
        Assert.Equal(5m, quote.TaxRate);
        Assert.Equal(3m, quote.TaxAmount);
        Assert.Equal(63m, quote.TotalAmount);
        Assert.Equal(FakeRoutingService.DefaultDistanceKm, quote.DistanceKm);
        Assert.Equal(FakeRoutingService.DefaultDistanceKm * TestSettings.CustomerRatePerKm, quote.EstimatedDeliveryFee);
        Assert.Equal(63m + quote.EstimatedDeliveryFee, quote.EstimatedGrandTotal);
    }

    [Fact]
    public async Task Handle_ShouldNotCreateAnyOrder_AndNotNotifyAnyone()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        await Mediator.Send(
            new GetOrderQuoteQuery(menu.Restaurant.Id, addressId, [new PlaceFoodOrderItem(menu.Item.Id, 1, [])]),
            CancellationToken.None);

        Assert.False(await Context.TbServiceRequests.AsNoTracking().AnyAsync());
        Assert.False(await Context.TbOrderItems.AsNoTracking().AnyAsync());
        Assert.Empty(Notifier.Records);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);

        var result = await Mediator.Send(
            new GetOrderQuoteQuery(menu.Restaurant.Id, addressId, [new PlaceFoodOrderItem(menu.Item.Id, 1, [])]),
            CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantClosed_WhenRestaurantIsClosed()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, approved: true, open: false);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var result = await Mediator.Send(
            new GetOrderQuoteQuery(menu.Restaurant.Id, addressId, [new PlaceFoodOrderItem(menu.Item.Id, 1, [])]),
            CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantClosed);
    }

    [Fact]
    public async Task Handle_ShouldReturnAddressNotFound_WhenAddressBelongsToAnotherUser()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var othersAddress = await ServiceRequestTestData.SeedAddressAsync(Context, TestUsers.Customer2.Id);
        AsCustomer();

        var result = await Mediator.Send(
            new GetOrderQuoteQuery(menu.Restaurant.Id, othersAddress, [new PlaceFoodOrderItem(menu.Item.Id, 1, [])]),
            CancellationToken.None);

        AssertError(result, ApplicationErrors.AddressNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnBelowMinimum_WhenItemsTotalIsTooLow()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, itemPrice: 5m);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var result = await Mediator.Send(
            new GetOrderQuoteQuery(menu.Restaurant.Id, addressId, [new PlaceFoodOrderItem(menu.Item.Id, 1, [])]),
            CancellationToken.None);

        AssertError(result, ApplicationErrors.BelowMinimumOrderAmount);
    }

    [Fact]
    public async Task Handle_ShouldReturnDistanceTooLong_WhenAddressIsTooFar()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        Routing.DistanceKm = TestSettings.MaxDistanceKm + 1m;
        AsCustomer();

        var result = await Mediator.Send(
            new GetOrderQuoteQuery(menu.Restaurant.Id, addressId, [new PlaceFoodOrderItem(menu.Item.Id, 1, [])]),
            CancellationToken.None);

        AssertError(result, ApplicationErrors.DeliveryDistanceTooLong);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenQuantityIsTooLarge()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var addressId = await ServiceRequestTestData.SeedAddressAsync(Context);
        AsCustomer();

        var result = await Mediator.Send(
            new GetOrderQuoteQuery(menu.Restaurant.Id, addressId, [new PlaceFoodOrderItem(menu.Item.Id, 100, [])]),
            CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Contains("Quantity", result.TopError.Code);
    }
}
