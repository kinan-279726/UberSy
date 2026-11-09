using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.ServiceRequests.Commands.OutForDelivery;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.ServiceRequests;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.OutForDelivery;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class OutForDelivaryCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    private async Task<SeededOrder> SeedPreparingOrderForDriver1Async(ServiceRequestStatus status = ServiceRequestStatus.Preparing)
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var driver = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id, onDelivery: true);

        return await ServiceRequestTestData.SeedOrderAsync(
            Context, menu, status: status, driverId: driver.Id,
            deliveryFee: 7500m, driverEarning: 6000m, commission: 500m);
    }

    [Fact]
    public async Task Handle_ShouldMoveOrderToOutForDelivery_WhenAssignedDriverPicksItUp()
    {
        var order = await SeedPreparingOrderForDriver1Async();
        AsDriver1();

        var result = await Mediator.Send(new OutForDelivaryCommand(order.Id), CancellationToken.None);

        AssertSuccess(result);
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Equal(ServiceRequestStatus.OutForDelivery, saved.IdNavigation.Status);
    }

    [Fact]
    public async Task Handle_ShouldNotifyCustomer()
    {
        var order = await SeedPreparingOrderForDriver1Async();
        AsDriver1();

        await Mediator.Send(new OutForDelivaryCommand(order.Id), CancellationToken.None);

        var notification = Assert.Single(Notifier.Of(nameof(IUserNotifier.NotifyServiceOutForDelivery)));
        Assert.Equal(order.CustomerId, notification.RecipientId);
        Assert.Equal(order.Id, notification.ServiceRequestId);
    }

    [Fact]
    public async Task Handle_ShouldReturnInvalidTransition_WhenOrderIsAlreadyOutForDelivery()
    {
        var order = await SeedPreparingOrderForDriver1Async(ServiceRequestStatus.OutForDelivery);
        AsDriver1();

        var result = await Mediator.Send(new OutForDelivaryCommand(order.Id), CancellationToken.None);

        AssertError(result, ServiceRequestErrors.InvalidStatusTransition);
    }

    [Fact]
    public async Task Handle_ShouldReturnInvalidTransition_WhenOrderIsStillPending()
    {
        var order = await SeedPreparingOrderForDriver1Async(ServiceRequestStatus.Pending);
        AsDriver1();

        var result = await Mediator.Send(new OutForDelivaryCommand(order.Id), CancellationToken.None);

        AssertError(result, ServiceRequestErrors.InvalidStatusTransition);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenOrderMissing()
    {
        await SeedPreparingOrderForDriver1Async();
        AsDriver1();

        var result = await Mediator.Send(new OutForDelivaryCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.ServiceRequestNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReject_WhenOrderBelongsToAnotherDriver()
    {
        // KNOWN BUG (security): the handler only checks that the caller has SOME order (AnyAsync(o => o.DriverId == me))
        // and then loads the requested service request without checking its driver, so any driver that has an order can
        // flip any other driver's order to OutForDelivery. Load the food order by (Id == request AND DriverId == me).
        var order = await SeedPreparingOrderForDriver1Async();
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, TestUsers.Owner2.Id);
        var driver2 = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver2.Id, onDelivery: true);
        await ServiceRequestTestData.SeedOrderAsync(
            Context, menu, status: ServiceRequestStatus.Preparing, driverId: driver2.Id,
            deliveryFee: 7500m, driverEarning: 6000m, commission: 500m);
        AsDriver2();

        var result = await Mediator.Send(new OutForDelivaryCommand(order.Id), CancellationToken.None);

        Assert.True(result.IsError, "A driver must not be able to update an order assigned to another driver.");
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Equal(ServiceRequestStatus.Preparing, saved.IdNavigation.Status);
    }

    [Fact]
    public async Task Handle_ShouldReject_WhenDriverHasNoOrders()
    {
        var order = await SeedPreparingOrderForDriver1Async();
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver2.Id);
        AsDriver2();

        var result = await Mediator.Send(new OutForDelivaryCommand(order.Id), CancellationToken.None);

        Assert.True(result.IsError);
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Equal(ServiceRequestStatus.Preparing, saved.IdNavigation.Status);
    }

    [Fact]
    public async Task Handle_ShouldReturnDriverNotFound_WhenUserIsNotADriver()
    {
        var order = await SeedPreparingOrderForDriver1Async();
        AsCustomer();

        var result = await Mediator.Send(new OutForDelivaryCommand(order.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.DriverNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var order = await SeedPreparingOrderForDriver1Async();

        var result = await Mediator.Send(new OutForDelivaryCommand(order.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }
}
