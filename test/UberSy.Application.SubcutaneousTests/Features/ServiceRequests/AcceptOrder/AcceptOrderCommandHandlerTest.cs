using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.ServiceRequests.Commands.AcceptFoodOrder;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.ServiceRequests;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.AcceptOrder;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class AcceptOrderCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldMoveOrderToPreparing_WhenRestaurantOwnerAccepts()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        AsOwner1();

        var result = await Mediator.Send(new AcceptOrderCommand(order.Id), CancellationToken.None);

        AssertSuccess(result);
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Equal(ServiceRequestStatus.Preparing, saved.IdNavigation.Status);
    }

    [Fact]
    public async Task Handle_ShouldNotifyCustomerThatRestaurantAccepted()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        AsOwner1();

        await Mediator.Send(new AcceptOrderCommand(order.Id), CancellationToken.None);

        var notification = Assert.Single(Notifier.Of(nameof(IUserNotifier.NotifyServiceAcceptedByRestaurant)));
        Assert.Equal(order.CustomerId, notification.RecipientId);
        Assert.Equal(order.Id, notification.ServiceRequestId);
    }

    [Fact]
    public async Task Handle_ShouldAllowManager()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        AsManager();

        var result = await Mediator.Send(new AcceptOrderCommand(order.Id), CancellationToken.None);

        AssertSuccess(result);
    }

    [Fact]
    public async Task Handle_ShouldOfferOrderToNearestOnlineDrivers_UpToBatchSize()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);

        // Restaurant is at (33.5138, 36.2765); the batch size is 2 in the test settings
        var near = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id, latitude: 33.5140);
        var middle = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver2.Id, latitude: 33.5200);
        var far = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver3.Id, latitude: 33.6000);
        AsOwner1();

        var result = await Mediator.Send(new AcceptOrderCommand(order.Id), CancellationToken.None);

        AssertSuccess(result);

        var offeredDriverIds = await Context.TbServiceRequestOffers.AsNoTracking()
            .Where(o => o.ServiceRequestId == order.Id)
            .Select(o => o.DriverId)
            .ToListAsync();

        Assert.Equal(2, offeredDriverIds.Count);
        Assert.Contains(near.Id, offeredDriverIds);
        Assert.Contains(middle.Id, offeredDriverIds);
        Assert.DoesNotContain(far.Id, offeredDriverIds);

        var notifiedDrivers = Notifier.Of(nameof(IDriverNotifier.NotifyOfferAsync)).Select(n => n.RecipientId).ToList();
        Assert.Equal(2, notifiedDrivers.Count);
        Assert.Contains(near.Id, notifiedDrivers);
        Assert.Contains(middle.Id, notifiedDrivers);
    }

    [Fact]
    public async Task Handle_ShouldSucceedWithoutOffers_WhenNoDriverCanReceiveOffers()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);

        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id, online: false);
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver2.Id, walletBalance: 0m);
        await ServiceRequestTestData.SeedDispatchableDriverAsync(
            Context, TestUsers.Driver3.Id, vehicleType: UberSy.Domain.DeliveryRule.Enums.VehicleType.Car);
        AsOwner1();

        var result = await Mediator.Send(new AcceptOrderCommand(order.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.False(await Context.TbServiceRequestOffers.AsNoTracking().AnyAsync(o => o.ServiceRequestId == order.Id));
        Assert.Empty(Notifier.Of(nameof(IDriverNotifier.NotifyOfferAsync)));
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenOrderMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(new AcceptOrderCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.ServiceRequestNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenAnotherOwner()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, TestUsers.Owner1.Id);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        AsOwner2();

        var result = await Mediator.Send(new AcceptOrderCommand(order.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Equal(ServiceRequestStatus.Pending, saved.IdNavigation.Status);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenCustomer()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        AsCustomer();

        var result = await Mediator.Send(new AcceptOrderCommand(order.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);

        var result = await Mediator.Send(new AcceptOrderCommand(order.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnInvalidTransition_WhenAlreadyPreparing()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Preparing);
        AsOwner1();

        var result = await Mediator.Send(new AcceptOrderCommand(order.Id), CancellationToken.None);

        AssertError(result, ServiceRequestErrors.InvalidStatusTransition);
        Assert.Empty(Notifier.Of(nameof(IUserNotifier.NotifyServiceAcceptedByRestaurant)));
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyCancelled_WhenOrderWasCancelled()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Cancelled);
        AsOwner1();

        var result = await Mediator.Send(new AcceptOrderCommand(order.Id), CancellationToken.None);

        AssertError(result, ServiceRequestErrors.ServiceRequestAlreadyCancelled);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyDelivered_WhenOrderWasDelivered()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Delivered);
        AsOwner1();

        var result = await Mediator.Send(new AcceptOrderCommand(order.Id), CancellationToken.None);

        AssertError(result, ServiceRequestErrors.ServiceRequestAlreadyDelivered);
    }

    [Fact]
    public async Task Handle_ShouldReject_WhenResponseDeadlineHasPassed()
    {
        // KNOWN GAP: the handler never checks FoodOrder.ResponseTimeout. Between the deadline and the moment the
        // background timeout service cancels the order, the restaurant can still accept it.
        // Delete this test if the background service is the intended (and only) guard.
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(
            Context, menu, responseTimeout: DateTimeOffset.UtcNow.AddMinutes(-5));
        AsOwner1();

        var result = await Mediator.Send(new AcceptOrderCommand(order.Id), CancellationToken.None);

        Assert.True(result.IsError, "An order whose response deadline has passed must not be acceptable.");
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenIdEmpty()
    {
        AsOwner1();

        var result = await Mediator.Send(new AcceptOrderCommand(Guid.Empty), CancellationToken.None);

        AssertErrorCode(result, "ServiceRequestId");
    }
}
