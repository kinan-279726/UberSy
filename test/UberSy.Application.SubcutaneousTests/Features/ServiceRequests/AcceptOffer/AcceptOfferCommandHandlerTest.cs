using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.ServiceRequests.Offers.Commands.AcceptOffer;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Application.SubcutaneousTests.Common.Fakes;
using UberSy.Domain.Drivers;
using UberSy.Domain.Drivers.Enums;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Domain.ServiceRequests.Offers;
using UberSy.Domain.ServiceRequests.Offers.Enums;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.AcceptOffer;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class AcceptOfferCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    private sealed record Setup(
        SeededOrder Order, TbDriver Driver1, TbDriver Driver2, TbServiceRequestOffer Offer1, TbServiceRequestOffer Offer2);

    // A Preparing order without a driver, offered to Driver1 and Driver2 (both Online with a wallet)
    private async Task<Setup> SeedOfferedOrderAsync(
        ServiceRequestStatus status = ServiceRequestStatus.Preparing,
        DateTimeOffset? offer1Timeout = null)
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: status, quantity: 2);
        var driver1 = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        var driver2 = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver2.Id, latitude: 33.5200);
        var offer1 = await ServiceRequestTestData.SeedOfferAsync(Context, order.Id, driver1.Id, responseTimeout: offer1Timeout);
        var offer2 = await ServiceRequestTestData.SeedOfferAsync(Context, order.Id, driver2.Id);

        return new Setup(order, driver1, driver2, offer1, offer2);
    }

    [Fact]
    public async Task Handle_ShouldAssignDriverToOrder_AndMarkOfferAccepted()
    {
        var s = await SeedOfferedOrderAsync();
        AsDriver1();

        var result = await Mediator.Send(new AcceptOfferCommand(s.Offer1.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(OfferStatus.Accepted, (await ServiceRequestTestData.LoadOfferAsync(Context, s.Offer1.Id)).Status);

        var order = await ServiceRequestTestData.LoadOrderAsync(Context, s.Order.Id);
        Assert.Equal(s.Driver1.Id, order.DriverId);
        Assert.Equal(ServiceRequestStatus.Preparing, order.IdNavigation.Status);
    }

    [Fact]
    public async Task Handle_ShouldRecordDeliveryFinancialsFromPricingAndCommission()
    {
        var s = await SeedOfferedOrderAsync();
        AsDriver1();

        await Mediator.Send(new AcceptOfferCommand(s.Offer1.Id), CancellationToken.None);

        var order = await ServiceRequestTestData.LoadOrderAsync(Context, s.Order.Id);
        Assert.Equal(FakeRoutingService.DefaultDistanceKm * TestSettings.CustomerRatePerKm, order.DeliveryFee);
        Assert.Equal(FakeRoutingService.DefaultDistanceKm * TestSettings.DriverRatePerKm, order.DriverEarning);
        Assert.Equal(5m, order.PlatformCommissionFromRestaurant); // 10% of 2 x 25
    }

    [Fact]
    public async Task Handle_ShouldPutDriverOnDelivery()
    {
        var s = await SeedOfferedOrderAsync();
        AsDriver1();

        await Mediator.Send(new AcceptOfferCommand(s.Offer1.Id), CancellationToken.None);

        var driver = await ServiceRequestTestData.LoadDriverAsync(Context, s.Driver1.Id);
        Assert.Equal(DriverStatus.OnDelivery, driver.Status);
    }

    [Fact]
    public async Task Handle_ShouldMarkOtherOffersLost_AndNotifyThoseDrivers()
    {
        var s = await SeedOfferedOrderAsync();
        AsDriver1();

        await Mediator.Send(new AcceptOfferCommand(s.Offer1.Id), CancellationToken.None);

        Assert.Equal(OfferStatus.LostToAnotherDriver, (await ServiceRequestTestData.LoadOfferAsync(Context, s.Offer2.Id)).Status);

        var notification = Assert.Single(Notifier.Of(nameof(IDriverNotifier.NotifyOfferLostAsync)));
        Assert.Equal(s.Driver2.Id, notification.RecipientId);
        Assert.Equal(s.Offer2.Id, notification.OfferId);
    }

    [Fact]
    public async Task Handle_ShouldNotifyCustomerThatDriverWasAssigned()
    {
        // KNOWN BUG: after saving, the handler looks the service request up with `s.Id == request.OfferId`
        // (an offer id) instead of `offer.ServiceRequestId`, so the lookup is always null and the customer is never notified.
        var s = await SeedOfferedOrderAsync();
        AsDriver1();

        await Mediator.Send(new AcceptOfferCommand(s.Offer1.Id), CancellationToken.None);

        var notification = Assert.Single(Notifier.Of(nameof(IUserNotifier.NotifyDriverAssigned)));
        Assert.Equal(s.Order.CustomerId, notification.RecipientId);
        Assert.Equal(s.Order.Id, notification.ServiceRequestId);
    }

    [Fact]
    public async Task Handle_ShouldRejectSecondDriver_WhenFirstDriverAlreadyAccepted()
    {
        var s = await SeedOfferedOrderAsync();

        AsDriver1();
        var first = await Mediator.Send(new AcceptOfferCommand(s.Offer1.Id), CancellationToken.None);

        AsDriver2();
        var second = await Mediator.Send(new AcceptOfferCommand(s.Offer2.Id), CancellationToken.None);

        AssertSuccess(first);
        AssertError(second, OfferErrors.OfferAlreadyRespondedTo);

        var order = await ServiceRequestTestData.LoadOrderAsync(Context, s.Order.Id);
        Assert.Equal(s.Driver1.Id, order.DriverId);
    }

    [Fact]
    public async Task Handle_ShouldReturnOfferAlreadyRespondedTo_WhenOfferWasRejected()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Preparing);
        var driver = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        var offer = await ServiceRequestTestData.SeedOfferAsync(Context, order.Id, driver.Id, OfferStatus.Rejected);
        AsDriver1();

        var result = await Mediator.Send(new AcceptOfferCommand(offer.Id), CancellationToken.None);

        AssertError(result, OfferErrors.OfferAlreadyRespondedTo);
    }

    [Fact]
    public async Task Handle_ShouldReject_WhenOfferHasExpired()
    {
        // KNOWN BUG: Accept() / the handler never compare ResponseTimeout with now (GetPendingOffer does), so a driver
        // can accept an expired offer until the background service flips its status to Expired.
        var s = await SeedOfferedOrderAsync(offer1Timeout: DateTimeOffset.UtcNow.AddMinutes(-5));
        AsDriver1();

        var result = await Mediator.Send(new AcceptOfferCommand(s.Offer1.Id), CancellationToken.None);

        Assert.True(result.IsError, "An expired offer must not be acceptable.");
        var order = await ServiceRequestTestData.LoadOrderAsync(Context, s.Order.Id);
        Assert.Null(order.DriverId);
    }

    [Fact]
    public async Task Handle_ShouldReject_WhenOrderWasCancelledMeanwhile()
    {
        // KNOWN BUG: offers that are still Sent are not closed when the order is cancelled. Accepting one assigns the
        // driver to a cancelled order and flips him to OnDelivery.
        var s = await SeedOfferedOrderAsync(ServiceRequestStatus.Cancelled);
        AsDriver1();

        var result = await Mediator.Send(new AcceptOfferCommand(s.Offer1.Id), CancellationToken.None);

        Assert.True(result.IsError, "An offer of a cancelled order must not be acceptable.");
        var driver = await ServiceRequestTestData.LoadDriverAsync(Context, s.Driver1.Id);
        Assert.NotEqual(DriverStatus.OnDelivery, driver.Status);
    }

    [Fact]
    public async Task Handle_ShouldKeepOfferAndOrderConsistent_WhenPricingFails()
    {
        // KNOWN BUG: if pricing fails inside DriverAcceptedOfferHandler (here: distance above the maximum) the handler only
        // logs and returns, so the command succeeds, the offer stays Accepted, and the order has no driver.
        // Invariant tested: the command succeeds if and only if the driver was really assigned.
        var s = await SeedOfferedOrderAsync();
        Routing.DistanceKm = TestSettings.MaxDistanceKm + 5m;
        AsDriver1();

        var result = await Mediator.Send(new AcceptOfferCommand(s.Offer1.Id), CancellationToken.None);

        var order = await ServiceRequestTestData.LoadOrderAsync(Context, s.Order.Id);
        Assert.Equal(result.IsSuccess, order.DriverId == s.Driver1.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnOfferNotFound_WhenOfferMissing()
    {
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        AsDriver1();

        var result = await Mediator.Send(new AcceptOfferCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.OfferNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenOfferBelongsToAnotherDriver()
    {
        var s = await SeedOfferedOrderAsync();
        AsDriver2();

        var result = await Mediator.Send(new AcceptOfferCommand(s.Offer1.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);
        Assert.Equal(OfferStatus.Sent, (await ServiceRequestTestData.LoadOfferAsync(Context, s.Offer1.Id)).Status);
    }

    [Fact]
    public async Task Handle_ShouldReturnDriverNotFound_WhenUserIsNotADriver()
    {
        var s = await SeedOfferedOrderAsync();
        AsCustomer();

        var result = await Mediator.Send(new AcceptOfferCommand(s.Offer1.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.DriverNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var s = await SeedOfferedOrderAsync();

        var result = await Mediator.Send(new AcceptOfferCommand(s.Offer1.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }
}
