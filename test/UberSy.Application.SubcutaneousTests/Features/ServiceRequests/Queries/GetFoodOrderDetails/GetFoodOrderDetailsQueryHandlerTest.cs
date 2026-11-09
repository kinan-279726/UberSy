using UberSy.Application.Common.Errors;
using UberSy.Application.Features.ServiceRequests.Queries.GetFoodOrderDetails;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.Queries.GetFoodOrderDetails;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetFoodOrderDetailsQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    private async Task<(SeededMenu Menu, SeededOrder Order)> SeedOrderAsync(Guid? driverId = null)
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, itemPrice: 25m, withAddOn: true, addOnPrice: 5m);
        var order = await ServiceRequestTestData.SeedOrderAsync(
            Context, menu, driverId: driverId, quantity: 2, includeAddOn: true, note: "No onions");

        return (menu, order);
    }

    [Fact]
    public async Task Handle_ShouldReturnOrderDetailsWithLinesAndAddOns_ForTheCustomer()
    {
        var (menu, order) = await SeedOrderAsync();
        AsCustomer();

        var result = await Mediator.Send(new GetFoodOrderDetailsQuery(order.Id), CancellationToken.None);

        AssertSuccess(result);
        var dto = result.Value;
        Assert.Equal(order.Id, dto.ServiceRequestId);
        Assert.True(dto.TrackingNumber > 0);
        Assert.Equal((byte)ServiceRequestStatus.Pending, dto.Status);
        Assert.Equal(order.TotalAmount, dto.TotalAmount);

        var line = Assert.Single(dto.Items);
        Assert.Equal(menu.Item.NameAr, line.ItemNameAr);
        Assert.Equal(menu.Item.NameEn, line.ItemNameEn);
        Assert.Equal(2, line.Quantity);
        Assert.Equal(25m, line.UnitPrice);
        Assert.Equal("No onions", line.Note);
        Assert.Equal(new[] { menu.AddOn!.NameAr }, line.AddOnNamesAr.ToArray());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenOrderMissing()
    {
        AsCustomer();

        var result = await Mediator.Send(new GetFoodOrderDetailsQuery(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.ServiceRequestNotFound);
    }

    [Fact]
    public async Task Handle_ShouldAllowTheRestaurantOwner()
    {
        var (_, order) = await SeedOrderAsync();
        AsOwner1();

        var result = await Mediator.Send(new GetFoodOrderDetailsQuery(order.Id), CancellationToken.None);

        AssertSuccess(result);
    }

    [Fact]
    public async Task Handle_ShouldAllowTheAssignedDriver()
    {
        var driver = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id, onDelivery: true);
        var (_, order) = await SeedOrderAsync(driver.Id);
        AsDriver1();

        var result = await Mediator.Send(new GetFoodOrderDetailsQuery(order.Id), CancellationToken.None);

        AssertSuccess(result);
    }

    [Fact]
    public async Task Handle_ShouldAllowTheManager()
    {
        var (_, order) = await SeedOrderAsync();
        AsManager();

        var result = await Mediator.Send(new GetFoodOrderDetailsQuery(order.Id), CancellationToken.None);

        AssertSuccess(result);
    }

    [Fact]
    public async Task Handle_ShouldDenyAnotherCustomer()
    {
        // KNOWN BUG (security / IDOR): the handler has no authorization at all, so any caller who knows (or guesses)
        // a service request id can read the order. Only the customer, the restaurant owner, the assigned driver and
        // the manager should be allowed.
        var (_, order) = await SeedOrderAsync();
        AsCustomer2();

        var result = await Mediator.Send(new GetFoodOrderDetailsQuery(order.Id), CancellationToken.None);

        Assert.True(result.IsError, "Another customer must not be able to read this order.");
    }

    [Fact]
    public async Task Handle_ShouldDenyAnotherRestaurantOwner()
    {
        var (_, order) = await SeedOrderAsync();
        AsOwner2();

        var result = await Mediator.Send(new GetFoodOrderDetailsQuery(order.Id), CancellationToken.None);

        Assert.True(result.IsError, "A different restaurant's owner must not be able to read this order.");
    }

    [Fact]
    public async Task Handle_ShouldDenyAnotherDriver()
    {
        var assigned = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id, onDelivery: true);
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver2.Id);
        var (_, order) = await SeedOrderAsync(assigned.Id);
        AsDriver2();

        var result = await Mediator.Send(new GetFoodOrderDetailsQuery(order.Id), CancellationToken.None);

        Assert.True(result.IsError, "A driver that is not assigned to the order must not be able to read it.");
    }

    [Fact]
    public async Task Handle_ShouldDenyAnonymousCaller()
    {
        var (_, order) = await SeedOrderAsync();

        var result = await Mediator.Send(new GetFoodOrderDetailsQuery(order.Id), CancellationToken.None);

        Assert.True(result.IsError, "An unauthenticated caller must not be able to read an order.");
    }
}
