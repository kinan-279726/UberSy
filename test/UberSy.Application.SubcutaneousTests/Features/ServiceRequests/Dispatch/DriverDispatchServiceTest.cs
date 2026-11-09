using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Domain.ServiceRequests.Offers.Enums;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.Dispatch;

/// <summary>
/// Calls IDriverDispatchService directly in a FRESH scope, exactly like the background DriverOfferTimeoutService does.
/// KNOWN BUG: DispatchNextBatchAsync reads foodOrder.IdNavigation.Status but only Include()s Restaurant. When the service
/// request is not already tracked by the scope's DbContext (the background-service path) IdNavigation is null and the
/// call throws NullReferenceException, so re-dispatch after offer timeouts never works. Add .Include(o => o.IdNavigation).
/// (The same code works inside command handlers only because they already loaded IdNavigation into the same context.)
/// </summary>
[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class DriverDispatchServiceTest(WebAppFactory factory) : AppTestBase(factory)
{
    private async Task<Result<Updated>> DispatchAsync(Guid serviceRequestId)
    {
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IDriverDispatchService>();

        return await service.DispatchNextBatchAsync(serviceRequestId, CancellationToken.None);
    }

    private async Task<List<Guid>> OfferedDriverIdsAsync(Guid serviceRequestId)
        => await Context.TbServiceRequestOffers.AsNoTracking()
            .Where(o => o.ServiceRequestId == serviceRequestId)
            .Select(o => o.DriverId)
            .ToListAsync();

    [Fact]
    public async Task Dispatch_ShouldOfferToNearestDrivers_UpToBatchSize()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Preparing);
        var near = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id, latitude: 33.5140);
        var middle = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver2.Id, latitude: 33.5200);
        var far = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver3.Id, latitude: 33.6000);

        var result = await DispatchAsync(order.Id);

        AssertSuccess(result);
        var offered = await OfferedDriverIdsAsync(order.Id);
        Assert.Equal(2, offered.Count);
        Assert.Contains(near.Id, offered);
        Assert.Contains(middle.Id, offered);
        Assert.DoesNotContain(far.Id, offered);
        Assert.Equal(2, Notifier.Of(nameof(IDriverNotifier.NotifyOfferAsync)).Count);
    }

    [Fact]
    public async Task Dispatch_ShouldSkipDriversThatWereAlreadyOffered_ForTheNextBatch()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Preparing);
        var near = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id, latitude: 33.5140);
        var middle = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver2.Id, latitude: 33.5200);
        var far = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver3.Id, latitude: 33.6000);

        await DispatchAsync(order.Id);
        var second = await DispatchAsync(order.Id);

        AssertSuccess(second);
        var offered = await OfferedDriverIdsAsync(order.Id);
        Assert.Equal(3, offered.Count);
        Assert.Equal(3, offered.Distinct().Count());
        Assert.Contains(far.Id, offered);
    }

    [Fact]
    public async Task Dispatch_ShouldReturnNoAvailableDriver_WhenEveryoneWasAlreadyOffered()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Preparing);
        var driver = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        await ServiceRequestTestData.SeedOfferAsync(Context, order.Id, driver.Id, OfferStatus.Expired);

        var result = await DispatchAsync(order.Id);

        AssertError(result, ApplicationErrors.NoAvailableDriverFound);
    }

    [Fact]
    public async Task Dispatch_ShouldIgnoreOfflineDriversDriversWithoutBalanceAndCars()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Preparing);
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id, online: false);
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver2.Id, walletBalance: 0m);
        await ServiceRequestTestData.SeedDispatchableDriverAsync(
            Context, TestUsers.Driver3.Id, vehicleType: UberSy.Domain.DeliveryRule.Enums.VehicleType.Car);

        var result = await DispatchAsync(order.Id);

        AssertError(result, ApplicationErrors.NoAvailableDriverFound);
        Assert.Empty(await OfferedDriverIdsAsync(order.Id));
    }

    [Fact]
    public async Task Dispatch_ShouldDoNothing_WhenOrderAlreadyHasADriver()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var assigned = await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id, onDelivery: true);
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver2.Id);
        var order = await ServiceRequestTestData.SeedOrderAsync(
            Context, menu, status: ServiceRequestStatus.Preparing, driverId: assigned.Id);

        var result = await DispatchAsync(order.Id);

        AssertSuccess(result);
        Assert.Empty(await OfferedDriverIdsAsync(order.Id));
    }

    [Fact]
    public async Task Dispatch_ShouldDoNothing_WhenOrderIsNotPreparing()
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var order = await ServiceRequestTestData.SeedOrderAsync(Context, menu, status: ServiceRequestStatus.Pending);
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);

        var result = await DispatchAsync(order.Id);

        AssertSuccess(result);
        Assert.Empty(await OfferedDriverIdsAsync(order.Id));
    }

    [Fact]
    public async Task Dispatch_ShouldReturnNotFound_WhenOrderMissing()
    {
        var result = await DispatchAsync(Guid.NewGuid());

        AssertError(result, ApplicationErrors.ServiceRequestNotFound);
    }
}
