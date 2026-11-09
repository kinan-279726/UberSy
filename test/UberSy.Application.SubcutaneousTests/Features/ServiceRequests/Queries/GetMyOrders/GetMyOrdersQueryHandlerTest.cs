using UberSy.Application.Common.Errors;
using UberSy.Application.Features.ServiceRequests.Queries.GetMyOrders;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.Queries.GetMyOrders;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetMyOrdersQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldReturnOnlyTheCurrentCustomersOrders()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var mine1 = await ServiceRequestTestData.SeedOrderAsync(Context, menu, TestUsers.Customer.Id);
        var mine2 = await ServiceRequestTestData.SeedOrderAsync(Context, menu, TestUsers.Customer.Id);
        await ServiceRequestTestData.SeedOrderAsync(Context, menu, TestUsers.Customer2.Id);
        AsCustomer();

        var result = await Mediator.Send(new GetMyOrdersQuery(null), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(2, result.Value.TotalCount);
        Assert.Equal(
            new[] { mine1.Id, mine2.Id }.Order().ToArray(),
            result.Value.Items.Select(i => i.ServiceRequestId).Order().ToArray());
    }

    [Fact]
    public async Task Handle_ShouldReturnNewestFirst()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var oldest = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        var middle = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        var newest = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        await ServiceRequestTestData.BackdateAsync(Context, oldest.Id, DateTimeOffset.UtcNow.AddHours(-3));
        await ServiceRequestTestData.BackdateAsync(Context, middle.Id, DateTimeOffset.UtcNow.AddHours(-2));
        await ServiceRequestTestData.BackdateAsync(Context, newest.Id, DateTimeOffset.UtcNow.AddHours(-1));
        AsCustomer();

        var result = await Mediator.Send(new GetMyOrdersQuery(null), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(new[] { newest.Id, middle.Id, oldest.Id }, result.Value.Items.Select(i => i.ServiceRequestId).ToArray());
    }

    [Fact]
    public async Task Handle_ShouldFilterByStatus()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Pending);
        var cancelled = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Cancelled);
        AsCustomer();

        var result = await Mediator.Send(new GetMyOrdersQuery(ServiceRequestStatus.Cancelled), CancellationToken.None);

        AssertSuccess(result);
        var item = Assert.Single(result.Value.Items);
        Assert.Equal(cancelled.Id, item.ServiceRequestId);
        Assert.Equal(ServiceRequestStatus.Cancelled, item.Status);
    }

    [Fact]
    public async Task Handle_ShouldMapOrderFields()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, itemPrice: 25m);
        var order = await ServiceRequestTestData.SeedOrderAsync(
            Context, menu, quantity: 2, deliveryFee: 7500m, driverEarning: 6000m, commission: 5m);
        AsCustomer();

        var result = await Mediator.Send(new GetMyOrdersQuery(null), CancellationToken.None);

        AssertSuccess(result);
        var dto = Assert.Single(result.Value.Items);
        Assert.Equal(order.Id, dto.ServiceRequestId);
        Assert.True(dto.TrackingNumber > 0);
        Assert.Equal(order.TotalAmount, dto.TotalAmount);
        Assert.Equal(7500m, dto.DeliveryFee);
        Assert.Equal(order.TotalAmount + 7500m, dto.PayableAmount);
        Assert.Equal(menu.Restaurant.Id, dto.RestaurantId);
        Assert.Equal(menu.Restaurant.NameAr, dto.RestaurantNameAr);
        Assert.Equal(menu.Restaurant.NameEn, dto.RestaurantNameEn);
        Assert.Equal(1, dto.ItemsCount);
    }

    [Fact]
    public async Task Handle_ShouldPageResults()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        for (var i = 0; i < 5; i++) await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        AsCustomer();

        var result = await Mediator.Send(new GetMyOrdersQuery(null, PageNumber: 3, PageSize: 2), CancellationToken.None);

        AssertSuccess(result);
        Assert.Single(result.Value.Items);
        Assert.Equal(5, result.Value.TotalCount);
        Assert.Equal(3, result.Value.TotalPages);
        Assert.True(result.Value.HasPreviousPage);
        Assert.False(result.Value.HasNextPage);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPage_WhenCustomerHasNoOrders()
    {
        AsCustomer();

        var result = await Mediator.Send(new GetMyOrdersQuery(null), CancellationToken.None);

        AssertSuccess(result);
        Assert.Empty(result.Value.Items);
        Assert.Equal(0, result.Value.TotalCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var result = await Mediator.Send(new GetMyOrdersQuery(null), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(-1, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 51)]
    public async Task Handle_ShouldReturnValidationError_WhenPagingIsInvalid(int pageNumber, int pageSize)
    {
        // KNOWN GAP: GetMyOrdersQuery has no validator (GetRestaurantOrdersQuery has one: page >= 1, size 1..50).
        // PageNumber 0 gives a negative Skip (database error) and PageSize is unbounded.
        AsCustomer();

        var result = await Mediator.Send(new GetMyOrdersQuery(null, pageNumber, pageSize), CancellationToken.None);

        Assert.True(result.IsError);
    }
}
