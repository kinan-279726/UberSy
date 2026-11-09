using UberSy.Application.Common.Errors;
using UberSy.Application.Features.ServiceRequests.Queries.GetRestaurantOrders;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.Queries.GetRestaurantOrders;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetRestaurantOrdersQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldReturnOnlyOrdersOfThatRestaurant()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, TestUsers.Owner1.Id);
        var otherMenu = await ServiceRequestTestData.SeedMenuAsync(Context, TestUsers.Owner2.Id);
        var mine = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        await ServiceRequestTestData.SeedOrderAsync(Context, otherMenu);
        AsOwner1();

        var result = await Mediator.Send(new GetRestaurantOrdersQuery(menu.Restaurant.Id, null), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(mine.Id, Assert.Single(result.Value.Items).ServiceRequestId);
        Assert.Equal(1, result.Value.TotalCount);
    }

    [Fact]
    public async Task Handle_ShouldMapOrderFields()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, quantity: 2);
        AsOwner1();

        var result = await Mediator.Send(new GetRestaurantOrdersQuery(menu.Restaurant.Id, null), CancellationToken.None);

        AssertSuccess(result);
        var dto = Assert.Single(result.Value.Items);
        Assert.Equal(order.Id, dto.ServiceRequestId);
        Assert.True(dto.TrackingNumber > 0);
        Assert.Equal(ServiceRequestStatus.Pending, dto.Status);
        Assert.Equal(order.TotalAmount, dto.TotalAmount);
        Assert.Equal(1, dto.ItemsCount);
    }

    [Fact]
    public async Task Handle_ShouldFilterByStatus()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Pending);
        var preparing = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Preparing);
        AsOwner1();

        var result = await Mediator.Send(
            new GetRestaurantOrdersQuery(menu.Restaurant.Id, ServiceRequestStatus.Preparing), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(preparing.Id, Assert.Single(result.Value.Items).ServiceRequestId);
    }

    [Fact]
    public async Task Handle_ShouldReturnNewestFirst_AndPage()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var older = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        var newer = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        await ServiceRequestTestData.BackdateAsync(Context, older.Id, DateTimeOffset.UtcNow.AddHours(-2));
        await ServiceRequestTestData.BackdateAsync(Context, newer.Id, DateTimeOffset.UtcNow.AddHours(-1));
        AsOwner1();

        var page1 = await Mediator.Send(
            new GetRestaurantOrdersQuery(menu.Restaurant.Id, null, PageNumber: 1, PageSize: 1), CancellationToken.None);
        var page2 = await Mediator.Send(
            new GetRestaurantOrdersQuery(menu.Restaurant.Id, null, PageNumber: 2, PageSize: 1), CancellationToken.None);

        AssertSuccess(page1);
        AssertSuccess(page2);
        Assert.Equal(newer.Id, Assert.Single(page1.Value.Items).ServiceRequestId);
        Assert.Equal(older.Id, Assert.Single(page2.Value.Items).ServiceRequestId);
        Assert.Equal(2, page1.Value.TotalCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenAnotherOwner()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, TestUsers.Owner1.Id);
        await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        AsOwner2();

        var result = await Mediator.Send(new GetRestaurantOrdersQuery(menu.Restaurant.Id, null), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenRestaurantMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(new GetRestaurantOrdersQuery(Guid.NewGuid(), null), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);

        var result = await Mediator.Send(new GetRestaurantOrdersQuery(menu.Restaurant.Id, null), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenPageSizeIsTooLarge()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        AsOwner1();

        var result = await Mediator.Send(
            new GetRestaurantOrdersQuery(menu.Restaurant.Id, null, PageSize: 51), CancellationToken.None);

        AssertErrorCode(result, "PageSize");
    }
}
