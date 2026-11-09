using UberSy.Application.Common.Errors;
using UberSy.Application.Features.ServiceRequests.Queries.GetPendingOffer;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Application.SubcutaneousTests.Common.Fakes;
using UberSy.Domain.Drivers;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Domain.ServiceRequests.Offers;
using UberSy.Domain.ServiceRequests.Offers.Enums;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.Queries.GetPendingOffer;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetPendingOfferQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    private async Task<(SeededMenu Menu, SeededOrder Order, TbDriver Driver, TbServiceRequestOffer Offer)> SeedAsync(
        OfferStatus status = OfferStatus.Sent, DateTimeOffset? timeout = null)
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context, itemPrice: 25m, withAddOn: true, addOnPrice: 5m);
        var order = await ServiceRequestTestData.SeedOrderAsync(
            Context, menu, status: ServiceRequestStatus.Preparing, quantity: 2, includeAddOn: true, note: "Extra sauce");
        var driver = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        var offer = await ServiceRequestTestData.SeedOfferAsync(Context, order.Id, driver.Id, status, timeout);

        return (menu, order, driver, offer);
    }

    [Fact]
    public async Task Handle_ShouldReturnOfferDetailsForTheDriver()
    {
        var (menu, order, _, offer) = await SeedAsync();
        AsDriver1();

        var result = await Mediator.Send(new GetPendingOfferQuery(offer.Id), CancellationToken.None);

        AssertSuccess(result);
        var dto = result.Value;
        Assert.Equal(offer.Id, dto.OfferId);
        Assert.Equal(order.Id, dto.ServiceRequestId);
        Assert.True(dto.TrackingNumber > 0);

        Assert.Equal(menu.Restaurant.NameAr, dto.Restaurant.NameAr);
        Assert.Equal(menu.Restaurant.NameEn, dto.Restaurant.NameEn);
        Assert.Equal(33.5138, dto.Restaurant.Latitude, 4);
        Assert.Equal(36.2765, dto.Restaurant.Longitude, 4);

        Assert.NotNull(dto.Customer);
        Assert.Equal("Home", dto.Customer!.NameAr);               // address label
        Assert.Equal("King Faisal Street", dto.Customer.Street);
        Assert.Equal("Damascus", dto.Customer.City);
        Assert.Equal("1", dto.Customer.Floor);

        var line = Assert.Single(dto.Items);
        Assert.Equal(menu.Item.NameAr, line.NameAr);
        Assert.Equal(2, line.Quantity);
        Assert.Equal("Extra sauce", line.Note);
        Assert.Equal(new[] { menu.AddOn!.NameAr }, line.AddOnNamesAr.ToArray());
    }

    [Fact]
    public async Task Handle_ShouldCalculatePricesFromQuoteAndOrder()
    {
        var (_, order, _, offer) = await SeedAsync();
        AsDriver1();

        var result = await Mediator.Send(new GetPendingOfferQuery(offer.Id), CancellationToken.None);

        AssertSuccess(result);
        var dto = result.Value;
        var fee = FakeRoutingService.DefaultDistanceKm * TestSettings.CustomerRatePerKm;

        Assert.Equal(FakeRoutingService.DefaultDistanceKm, dto.DistanceKm);
        Assert.Equal(fee, dto.DeliveryFee);
        Assert.Equal(FakeRoutingService.DefaultDistanceKm * TestSettings.DriverRatePerKm, dto.DriverEarning);
        Assert.Equal(order.TotalAmount + fee, dto.CustomerPrice);
        Assert.Equal(54m, dto.RestaurantPrice);                  // items (25 + 5) * 2 = 60, minus 10% commission (6)
    }

    [Fact]
    public async Task Handle_ShouldReportRemainingSeconds()
    {
        var (_, _, _, offer) = await SeedAsync(timeout: DateTimeOffset.UtcNow.AddSeconds(45));
        AsDriver1();

        var result = await Mediator.Send(new GetPendingOfferQuery(offer.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.InRange(result.Value.SecondsRemaining, 1, 45);
    }

    [Theory]
    [InlineData(OfferStatus.Accepted)]
    [InlineData(OfferStatus.Rejected)]
    [InlineData(OfferStatus.Expired)]
    [InlineData(OfferStatus.LostToAnotherDriver)]
    public async Task Handle_ShouldReturnNoLongerAvailable_WhenOfferIsNotSent(OfferStatus status)
    {
        var (_, _, _, offer) = await SeedAsync(status);
        AsDriver1();

        var result = await Mediator.Send(new GetPendingOfferQuery(offer.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.OfferNoLongerAvailable);
    }

    [Fact]
    public async Task Handle_ShouldReturnNoLongerAvailable_WhenOfferTimedOut()
    {
        var (_, _, _, offer) = await SeedAsync(timeout: DateTimeOffset.UtcNow.AddMinutes(-1));
        AsDriver1();

        var result = await Mediator.Send(new GetPendingOfferQuery(offer.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.OfferNoLongerAvailable);
    }

    [Fact]
    public async Task Handle_ShouldReturnDeliveryDistanceTooLong_WhenAddressIsBeyondMaximum()
    {
        var (_, _, _, offer) = await SeedAsync();
        Routing.DistanceKm = TestSettings.MaxDistanceKm + 5m;
        AsDriver1();

        var result = await Mediator.Send(new GetPendingOfferQuery(offer.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.DeliveryDistanceTooLong);
    }

    [Fact]
    public async Task Handle_ShouldReturnOfferNotFound_WhenOfferMissing()
    {
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        AsDriver1();

        var result = await Mediator.Send(new GetPendingOfferQuery(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.OfferNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenOfferBelongsToAnotherDriver()
    {
        var (_, _, _, offer) = await SeedAsync();
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver2.Id);
        AsDriver2();

        var result = await Mediator.Send(new GetPendingOfferQuery(offer.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);
    }

    [Fact]
    public async Task Handle_ShouldReturnDriverNotFound_WhenUserIsNotADriver()
    {
        var (_, _, _, offer) = await SeedAsync();
        AsCustomer();

        var result = await Mediator.Send(new GetPendingOfferQuery(offer.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.DriverNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var (_, _, _, offer) = await SeedAsync();

        var result = await Mediator.Send(new GetPendingOfferQuery(offer.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }
}
