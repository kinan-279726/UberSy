using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.ServiceRequests.Commands.MarkDelivered;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Drivers;
using UberSy.Domain.Drivers.Enums;
using UberSy.Domain.Drivers.Wallet.Enums;
using UberSy.Domain.ServiceRequests;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.MarkDelivered;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class MarkDeliveredCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    // Platform due = commission + (delivery fee - driver earning) = 500 + (7500 - 6000) = 2000
    private const decimal WalletBalance = 10_000m;
    private const decimal ExpectedDue = 2_000m;

    private async Task<(SeededOrder Order, TbDriver Driver)> SeedOutForDeliveryOrderAsync(
        decimal deliveryFee = 7500m,
        decimal driverEarning = 6000m,
        decimal commission = 500m,
        ServiceRequestStatus status = ServiceRequestStatus.OutForDelivery)
    {
        var menu = await ServiceRequestTestData.SeedMenuAsync(Context);
        var driver = await ServiceRequestTestData.SeedDispatchableDriverAsync(
            Context, TestUsers.Driver1.Id, walletBalance: WalletBalance, onDelivery: true);
        var order = await ServiceRequestTestData.SeedOrderAsync(
            Context, menu, status: status, driverId: driver.Id,
            deliveryFee: deliveryFee, driverEarning: driverEarning, commission: commission);

        return (order, driver);
    }

    [Fact]
    public async Task Handle_ShouldMarkOrderDelivered_AndFreeTheDriver()
    {
        var (order, driver) = await SeedOutForDeliveryOrderAsync();
        AsDriver1();

        var result = await Mediator.Send(new MarkDeliveredCommand(order.Id), CancellationToken.None);

        AssertSuccess(result);
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Equal(ServiceRequestStatus.Delivered, saved.IdNavigation.Status);

        var savedDriver = await ServiceRequestTestData.LoadDriverAsync(Context, driver.Id);
        Assert.Equal(DriverStatus.Online, savedDriver.Status);
    }

    [Fact]
    public async Task Handle_ShouldDeductPlatformDueFromDriverWallet_AndRecordTransaction()
    {
        var (order, driver) = await SeedOutForDeliveryOrderAsync();
        AsDriver1();

        await Mediator.Send(new MarkDeliveredCommand(order.Id), CancellationToken.None);

        var wallet = await Context.TbDriverWallets.AsNoTracking().FirstAsync(w => w.DriverId == driver.Id);
        Assert.Equal(WalletBalance - ExpectedDue, wallet.Balance);

        var transaction = await Context.TbDriverWalletTransactions.AsNoTracking()
            .SingleAsync(t => t.ServiceRequestId == order.Id);
        Assert.Equal(-ExpectedDue, transaction.Amount);
        Assert.Equal(WalletTransactionType.PlatformCommissionDeduction, transaction.Type);
        Assert.Equal(wallet.Id, transaction.DriverWalletId);
    }

    [Fact]
    public async Task Handle_ShouldNotifyCustomerThatOrderWasDelivered()
    {
        var (order, _) = await SeedOutForDeliveryOrderAsync();
        AsDriver1();

        await Mediator.Send(new MarkDeliveredCommand(order.Id), CancellationToken.None);

        var notification = Assert.Single(Notifier.Of(nameof(IUserNotifier.NotifyServiceDelivered)));
        Assert.Equal(order.CustomerId, notification.RecipientId);
        Assert.Equal(order.Id, notification.ServiceRequestId);
    }

    [Fact]
    public async Task Handle_ShouldNotDeductTwice_WhenCalledTwice()
    {
        var (order, driver) = await SeedOutForDeliveryOrderAsync();
        AsDriver1();

        var first = await Mediator.Send(new MarkDeliveredCommand(order.Id), CancellationToken.None);
        var second = await Mediator.Send(new MarkDeliveredCommand(order.Id), CancellationToken.None);

        AssertSuccess(first);
        Assert.True(second.IsError);
        Assert.Equal(1, await Context.TbDriverWalletTransactions.AsNoTracking().CountAsync(t => t.ServiceRequestId == order.Id));

        var wallet = await Context.TbDriverWallets.AsNoTracking().FirstAsync(w => w.DriverId == driver.Id);
        Assert.Equal(WalletBalance - ExpectedDue, wallet.Balance);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenPlatformDueIsZero()
    {
        // KNOWN BUG: when commission is 0 and the delivery fee equals the driver earning, platformDue is 0 and the handler
        // returns CommissionNegative AFTER changing the status but BEFORE saving, so the order can never be delivered.
        // A zero due should simply skip the wallet deduction.
        var (order, driver) = await SeedOutForDeliveryOrderAsync(deliveryFee: 6000m, driverEarning: 6000m, commission: 0m);
        AsDriver1();

        var result = await Mediator.Send(new MarkDeliveredCommand(order.Id), CancellationToken.None);

        AssertSuccess(result);
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Equal(ServiceRequestStatus.Delivered, saved.IdNavigation.Status);

        var wallet = await Context.TbDriverWallets.AsNoTracking().FirstAsync(w => w.DriverId == driver.Id);
        Assert.Equal(WalletBalance, wallet.Balance);
    }

    [Fact]
    public async Task Handle_ShouldReturnInvalidTransition_WhenOrderIsStillPreparing()
    {
        var (order, driver) = await SeedOutForDeliveryOrderAsync(status: ServiceRequestStatus.Preparing);
        AsDriver1();

        var result = await Mediator.Send(new MarkDeliveredCommand(order.Id), CancellationToken.None);

        AssertError(result, ServiceRequestErrors.InvalidStatusTransition);
        Assert.False(await Context.TbDriverWalletTransactions.AsNoTracking().AnyAsync());
        var savedDriver = await ServiceRequestTestData.LoadDriverAsync(Context, driver.Id);
        Assert.Equal(DriverStatus.OnDelivery, savedDriver.Status);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyDelivered_WhenOrderWasDelivered()
    {
        var (order, _) = await SeedOutForDeliveryOrderAsync(status: ServiceRequestStatus.Delivered);
        AsDriver1();

        var result = await Mediator.Send(new MarkDeliveredCommand(order.Id), CancellationToken.None);

        AssertError(result, ServiceRequestErrors.ServiceRequestAlreadyDelivered);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenOrderMissing()
    {
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver1.Id);
        AsDriver1();

        var result = await Mediator.Send(new MarkDeliveredCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.ServiceRequestNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenOrderBelongsToAnotherDriver()
    {
        var (order, _) = await SeedOutForDeliveryOrderAsync();
        await ServiceRequestTestData.SeedDispatchableDriverAsync(Context, TestUsers.Driver2.Id);
        AsDriver2();

        var result = await Mediator.Send(new MarkDeliveredCommand(order.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);
        var saved = await ServiceRequestTestData.LoadOrderAsync(Context, order.Id);
        Assert.Equal(ServiceRequestStatus.OutForDelivery, saved.IdNavigation.Status);
    }

    [Fact]
    public async Task Handle_ShouldReturnDriverNotFound_WhenUserIsNotADriver()
    {
        var (order, _) = await SeedOutForDeliveryOrderAsync();
        AsCustomer();

        var result = await Mediator.Send(new MarkDeliveredCommand(order.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.DriverNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var (order, _) = await SeedOutForDeliveryOrderAsync();

        var result = await Mediator.Send(new MarkDeliveredCommand(order.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }
}
