using UberSy.Application.Common.Errors;
using UberSy.Application.Features.ServiceRequests.Queries.GetMyDriverOrders;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.Queries.GetMyDriverOrders;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetMyDriverOrdersQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldReturnOnlyOrdersAssignedToTheCurrentDriver()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var driver1 = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        var driver2 = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver2.Id);
        var mine = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Preparing, driverId: driver1.Id);
        await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Preparing, driverId: driver2.Id);
        await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Pending);
        AsDriver1();

        var result = await Mediator.Send(new GetMyDriverOrdersQuery(null), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(mine.Id, Assert.Single(result.Value.Items).ServiceRequestId);
        Assert.Equal(1, result.Value.TotalCount);
    }

    [Fact]
    public async Task Handle_ShouldFilterByStatus()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var driver = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Preparing, driverId: driver.Id);
        var delivered = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Delivered, driverId: driver.Id);
        AsDriver1();

        var result = await Mediator.Send(new GetMyDriverOrdersQuery(ServiceRequestStatus.Delivered), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(delivered.Id, Assert.Single(result.Value.Items).ServiceRequestId);
    }

    [Fact]
    public async Task Handle_ShouldMapOrderFields()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var driver = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        var order = await ServiceRequestTestData.SeedOrderAsync(
            Context, menu, status: ServiceRequestStatus.Preparing, driverId: driver.Id,
            deliveryFee: 7500m, driverEarning: 6000m, commission: 5m);
        AsDriver1();

        var result = await Mediator.Send(new GetMyDriverOrdersQuery(null), CancellationToken.None);

        AssertSuccess(result);
        var dto = Assert.Single(result.Value.Items);
        Assert.Equal(order.Id, dto.ServiceRequestId);
        Assert.True(dto.TrackingNumber > 0);
        Assert.Equal(ServiceRequestStatus.Preparing, dto.Status);
        Assert.Equal(menu.Restaurant.NameAr, dto.RestaurantNameAr);
        Assert.Equal("King Faisal Street", dto.CustomerStreet);
        Assert.Equal("Damascus", dto.CustomerCity);
        Assert.Equal(order.TotalAmount + 7500m, dto.CashToCollect);
        Assert.Equal(7500m, dto.DeliveryFee);
        Assert.Equal(6000m, dto.DriverEarning);
    }

    [Fact]
    public async Task Handle_ShouldReturnNewestFirst_AndPage()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var driver = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        var older = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Delivered, driverId: driver.Id);
        var newer = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Delivered, driverId: driver.Id);
        await ServiceRequestTestData.BackdateAsync(Context, older.Id, DateTimeOffset.UtcNow.AddHours(-2));
        await ServiceRequestTestData.BackdateAsync(Context, newer.Id, DateTimeOffset.UtcNow.AddHours(-1));
        AsDriver1();

        var page1 = await Mediator.Send(new GetMyDriverOrdersQuery(null, PageNumber: 1, PageSize: 1), CancellationToken.None);

        AssertSuccess(page1);
        Assert.Equal(newer.Id, Assert.Single(page1.Value.Items).ServiceRequestId);
        Assert.Equal(2, page1.Value.TotalCount);
        Assert.True(page1.Value.HasNextPage);
    }

    [Fact]
    public async Task Handle_ShouldReturnDriverNotFound_WhenUserIsNotADriver()
    {
        AsCustomer();

        var result = await Mediator.Send(new GetMyDriverOrdersQuery(null), CancellationToken.None);

        AssertError(result, ApplicationErrors.DriverNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var result = await Mediator.Send(new GetMyDriverOrdersQuery(null), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(1, 0)]
    [InlineData(1, 51)]
    public async Task Handle_ShouldReturnValidationError_WhenPagingIsInvalid(int pageNumber, int pageSize)
    {
        // KNOWN GAP: no validator for GetMyDriverOrdersQuery (same problem as GetMyOrdersQuery).
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        AsDriver1();

        var result = await Mediator.Send(new GetMyDriverOrdersQuery(null, pageNumber, pageSize), CancellationToken.None);

        Assert.True(result.IsError);
    }
}
