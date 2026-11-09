using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.ServiceRequests.Commands.CancelService;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Drivers.Enums;
using UberSy.Domain.ServiceRequests;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Domain.ServiceRequests.Offers.Enums;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.CancelService;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class CancelServiceCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    private const string Reason = "Test";

    // ---------- customer ----------

    [Fact]
    public async Task Handle_ShouldCancelPendingOrder_WhenCustomerOwnsIt()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        AsCustomer();

        var result = await Mediator.Send(new CancelServiceCommand(order.Id, Reason), CancellationToken.None);

        AssertSuccess(result);
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Equal(ServiceRequestStatus.Cancelled, saved.IdNavigation.Status);
    }

    [Fact]
    public async Task Handle_ShouldNotifyRestaurant_WhenCustomerCancels()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        AsCustomer();

        await Mediator.Send(new CancelServiceCommand(order.Id, Reason), CancellationToken.None);

        var notification = Assert.Single(Notifier.Of(nameof(IRestaurantNotifier.NotifyOrderCancledAsync)));
        Assert.Equal(order.RestaurantId, notification.RecipientId);
        Assert.Equal(order.Id, notification.ServiceRequestId);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenAnotherCustomer()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, customerId: TestUsers.Customer.Id);
        AsCustomer2();

        var result = await Mediator.Send(new CancelServiceCommand(order.Id, Reason), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Equal(ServiceRequestStatus.Pending, saved.IdNavigation.Status);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenServiceMissing()
    {
        AsCustomer();

        var result = await Mediator.Send(new CancelServiceCommand(Guid.NewGuid(), Reason), CancellationToken.None);

        AssertError(result, ApplicationErrors.ServiceRequestNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        AsAnonymous();

        var result = await Mediator.Send(new CancelServiceCommand(order.Id, Reason), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Equal(ServiceRequestStatus.Pending, saved.IdNavigation.Status);
    }

    [Theory]
    [InlineData(ServiceRequestStatus.Preparing)]
    [InlineData(ServiceRequestStatus.OutForDelivery)]
    public async Task Handle_ShouldReturnOrderAlreadyAccepted_WhenCustomerCancelsAcceptedOrder(ServiceRequestStatus status)
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: status);
        AsCustomer();

        var result = await Mediator.Send(new CancelServiceCommand(order.Id, Reason), CancellationToken.None);

        AssertError(result, ApplicationErrors.OrderAlreadyAccepted);
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Equal(status, saved.IdNavigation.Status);
        Assert.Empty(Notifier.Of(nameof(IRestaurantNotifier.NotifyOrderCancledAsync)));
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyCancelled_WhenOrderWasAlreadyCancelled()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Cancelled);
        AsCustomer();

        var result = await Mediator.Send(new CancelServiceCommand(order.Id, Reason), CancellationToken.None);

        AssertError(result, ServiceRequestErrors.ServiceRequestAlreadyCancelled);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyDelivered_WhenOrderWasDelivered()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Delivered);
        AsCustomer();

        var result = await Mediator.Send(new CancelServiceCommand(order.Id, Reason), CancellationToken.None);

        AssertError(result, ServiceRequestErrors.ServiceRequestAlreadyDelivered);
    }

    // ---------- admin ----------

    [Fact]
    public async Task Handle_ShouldAllowAdmin_ToCancelAnotherUsersPendingOrder()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        AsManager();

        var result = await Mediator.Send(new CancelServiceCommand(order.Id, Reason), CancellationToken.None);

        AssertSuccess(result);
    }

    [Fact]
    public async Task Handle_ShouldAllowAdmin_ToCancelAcceptedOrder()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Preparing);
        AsManager();

        var result = await Mediator.Send(new CancelServiceCommand(order.Id, Reason), CancellationToken.None);

        AssertSuccess(result);
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Equal(ServiceRequestStatus.Cancelled, saved.IdNavigation.Status);
        Assert.Single(Notifier.Of(nameof(IRestaurantNotifier.NotifyOrderCancledAsync)));
    }

    // ---------- ServiceRequestCancelledHandler (runs inside the command) ----------

    [Fact]
    public async Task Handle_ShouldReleaseAssignedDriver_WhenAdminCancelsAcceptedOrder()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var driver = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id, onDelivery: true);
        var order = await ServiceRequestTestData.SeedOrderAsync(
            Context, menu, status: ServiceRequestStatus.Preparing, driverId: driver.Id);
        AsManager();

        var result = await Mediator.Send(new CancelServiceCommand(order.Id, Reason), CancellationToken.None);

        AssertSuccess(result);
        var savedDriver = await ServiceRequestTestData.LoadDriverAsync(Context, driver.Id);
        Assert.Equal(DriverStatus.Online, savedDriver.Status);
    }

    [Fact]
    public async Task Handle_ShouldExpireSentOffers_WhenOrderIsCancelled()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        var driver = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        var offer = await ServiceRequestTestData.SeedOfferAsync(Context, order.Id, driver.Id);
        AsCustomer();

        var result = await Mediator.Send(new CancelServiceCommand(order.Id, Reason), CancellationToken.None);

        AssertSuccess(result);
        var savedOffer = await ServiceRequestTestData.LoadOfferAsync(Context, offer.Id);
        Assert.Equal(OfferStatus.Expired, savedOffer.Status);
    }

    [Fact]
    public async Task Handle_ShouldNotifyDriversWithSentOffers_WhenOrderIsCancelled()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu);
        var driver = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        await ServiceRequestTestData.SeedOfferAsync(Context, order.Id, driver.Id);
        AsCustomer();

        await Mediator.Send(new CancelServiceCommand(order.Id, Reason), CancellationToken.None);

        var notification = Assert.Single(Notifier.Of(nameof(IDriverNotifier.NotifyOrderCanceldAsync)));
        Assert.Equal(driver.Id, notification.RecipientId);
    }
}
