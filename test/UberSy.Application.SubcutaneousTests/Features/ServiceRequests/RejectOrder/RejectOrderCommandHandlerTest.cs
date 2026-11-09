using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.ServiceRequests.Commands.RejectFoodOrder;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Drivers.Enums;
using UberSy.Domain.ServiceRequests;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.RejectOrder;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class RejectOrderCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldCancelOrderWithReason_WhenOwnerRejects()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        AsOwner1();

        var result = await Mediator.Send(new RejectOrderCommand(order.Id, "Out of stock"), CancellationToken.None);

        AssertSuccess(result);
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Equal(ServiceRequestStatus.Cancelled, saved.IdNavigation.Status);
        Assert.Equal("Out of stock", saved.IdNavigation.CancellationReason);
    }

    [Fact]
    public async Task Handle_ShouldNotifyCustomerThatRestaurantCancelled()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        AsOwner1();

        await Mediator.Send(new RejectOrderCommand(order.Id, "Out of stock"), CancellationToken.None);

        var notification = Assert.Single(Notifier.Of(nameof(IUserNotifier.NotifyServiceCancelledRestaurant)));
        Assert.Equal(order.CustomerId, notification.RecipientId);
        Assert.Equal(order.Id, notification.ServiceRequestId);
    }

    [Fact]
    public async Task Handle_ShouldAllowManager()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        AsManager();

        var result = await Mediator.Send(new RejectOrderCommand(order.Id, "Policy violation"), CancellationToken.None);

        AssertSuccess(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenOrderMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(new RejectOrderCommand(Guid.NewGuid(), "Out of stock"), CancellationToken.None);

        AssertError(result, ApplicationErrors.ServiceRequestNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenAnotherOwner()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, TestUsers.Owner1.Id);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        AsOwner2();

        var result = await Mediator.Send(new RejectOrderCommand(order.Id, "Out of stock"), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Equal(ServiceRequestStatus.Pending, saved.IdNavigation.Status);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);

        var result = await Mediator.Send(new RejectOrderCommand(order.Id, "Out of stock"), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyCancelled_WhenOrderWasCancelled()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Cancelled);
        AsOwner1();

        var result = await Mediator.Send(new RejectOrderCommand(order.Id, "Out of stock"), CancellationToken.None);

        AssertError(result, ServiceRequestErrors.ServiceRequestAlreadyCancelled);
        Assert.Empty(Notifier.Of(nameof(IUserNotifier.NotifyServiceCancelledRestaurant)));
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyDelivered_WhenOrderWasDelivered()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Delivered);
        AsOwner1();

        var result = await Mediator.Send(new RejectOrderCommand(order.Id, "Out of stock"), CancellationToken.None);

        AssertError(result, ServiceRequestErrors.ServiceRequestAlreadyDelivered);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenReasonTooLong()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        AsOwner1();

        var result = await Mediator.Send(new RejectOrderCommand(order.Id, new string('x', 101)), CancellationToken.None);

        AssertErrorCode(result, "Reason");
    }

    [Fact]
    public async Task Handle_ShouldReject_WhenOrderIsAlreadyOutForDelivery()
    {
        // KNOWN BUG: the restaurant can "reject" an order that is already out for delivery because the domain allows
        // Cancel from OutForDelivery. The restaurant should only be able to reject while the order is still Pending.
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var driver = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id, onDelivery: true);
        var order = await ServiceRequestTestData.SeedOrderAsync(
            Context, menu, status: ServiceRequestStatus.OutForDelivery, driverId: driver.Id);
        AsOwner1();

        var result = await Mediator.Send(new RejectOrderCommand(order.Id, "Changed my mind"), CancellationToken.None);

        Assert.True(result.IsError, "A restaurant must not be able to reject an order that is already out for delivery.");
    }

    [Fact]
    public async Task Handle_ShouldNotLeaveDriverOnDelivery_WhenAcceptedOrderIsRejected()
    {
        // KNOWN BUG: rejecting a Preparing order that already has a driver cancels the order but never frees the driver,
        // so the driver stays OnDelivery forever and can no longer go offline or receive offers.
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var driver = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id, onDelivery: true);
        var order = await ServiceRequestTestData.SeedOrderAsync(
            Context, menu, status: ServiceRequestStatus.Preparing, driverId: driver.Id);
        AsOwner1();

        var result = await Mediator.Send(new RejectOrderCommand(order.Id, "Kitchen closed"), CancellationToken.None);

        if (result.IsSuccess)
        {
            var savedDriver = await ServiceRequestTestData.LoadDriverAsync(Context, driver.Id);
            Assert.NotEqual(DriverStatus.OnDelivery, savedDriver.Status);
        }
    }
}
