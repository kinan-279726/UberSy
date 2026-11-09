using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.ServiceRequests.Commands.DriverCancelAfterAccepted;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Drivers;
using UberSy.Domain.Drivers.Enums;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Domain.ServiceRequests.Offers.Enums;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.ReleaseOrder;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class ReleaseOrderCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    // A Preparing order assigned to Driver1 (who is OnDelivery and holds an Accepted offer, as in the real flow)
    private async Task<(SeededOrder Order, TbDriver Driver)> SeedAssignedOrderAsync(
        ServiceRequestStatus status = ServiceRequestStatus.Preparing)
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var driver = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id, onDelivery: true);
        var order = await ServiceRequestTestData.SeedOrderAsync(
            Context, menu, status: status, driverId: driver.Id,
            deliveryFee: 7500m, driverEarning: 6000m, commission: 500m);
        await ServiceRequestTestData.SeedOfferAsync(Context, order.Id, driver.Id, OfferStatus.Accepted);

        return (order, driver);
    }

    [Fact]
    public async Task Handle_ShouldRemoveDriverFromOrder_WhenDriverReleasesBeforePickup()
    {
        var (order, driver) = await SeedAssignedOrderAsync();
        AsDriver1();

        var result = await Mediator.Send(new ReleaseOrderCommand(order.Id), CancellationToken.None);

        AssertSuccess(result);
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Null(saved.DriverId);
        Assert.Equal(ServiceRequestStatus.Preparing, saved.IdNavigation.Status);
    }

    [Fact]
    public async Task Handle_ShouldFreeTheDriver()
    {
        var (order, driver) = await SeedAssignedOrderAsync();
        AsDriver1();

        await Mediator.Send(new ReleaseOrderCommand(order.Id), CancellationToken.None);

        var savedDriver = await ServiceRequestTestData.LoadDriverAsync(Context, driver.Id);
        Assert.Equal(DriverStatus.Online, savedDriver.Status);
    }

    [Fact]
    public async Task Handle_ShouldReDispatchToOtherDrivers_AndNeverBackToTheReleasingDriver()
    {
        var (order, driver) = await SeedAssignedOrderAsync();
        var other = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver2.Id, latitude: 33.5200);
        AsDriver1();

        var result = await Mediator.Send(new ReleaseOrderCommand(order.Id), CancellationToken.None);

        AssertSuccess(result);

        var newOffers = await Context.TbServiceRequestOffers.AsNoTracking()
            .Where(o => o.ServiceRequestId == order.Id && o.Status == OfferStatus.Sent)
            .ToListAsync();

        Assert.Equal(other.Id, Assert.Single(newOffers).DriverId);

        var notification = Assert.Single(Notifier.Of(nameof(IDriverNotifier.NotifyOfferAsync)));
        Assert.Equal(other.Id, notification.RecipientId);
    }

    [Fact]
    public async Task Handle_ShouldReturnDriverNotFound_WhenUserIsNotADriver()
    {
        var (order, _) = await SeedAssignedOrderAsync();
        AsCustomer();

        var result = await Mediator.Send(new ReleaseOrderCommand(order.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.DriverNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var (order, _) = await SeedAssignedOrderAsync();

        var result = await Mediator.Send(new ReleaseOrderCommand(order.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenOrderMissing()
    {
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        AsDriver1();

        var result = await Mediator.Send(new ReleaseOrderCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.ServiceRequestNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenOrderBelongsToAnotherDriver()
    {
        var (order, driver) = await SeedAssignedOrderAsync();
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver2.Id);
        AsDriver2();

        var result = await Mediator.Send(new ReleaseOrderCommand(order.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Equal(driver.Id, saved.DriverId);
    }

    [Fact]
    public async Task Handle_ShouldReturnCannotRelease_WhenOrderIsAlreadyOutForDelivery()
    {
        var (order, driver) = await SeedAssignedOrderAsync(ServiceRequestStatus.OutForDelivery);
        AsDriver1();

        var result = await Mediator.Send(new ReleaseOrderCommand(order.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.CannotReleaseOrderAfterPickup);
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Equal(driver.Id, saved.DriverId);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenIdEmpty()
    {
        AsDriver1();

        var result = await Mediator.Send(new ReleaseOrderCommand(Guid.Empty), CancellationToken.None);

        AssertErrorCode(result, "ServiceRequestId");
    }
}
