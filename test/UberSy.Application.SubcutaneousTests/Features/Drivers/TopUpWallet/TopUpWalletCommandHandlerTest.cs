using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Drivers.Commands.TopUpWallet;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Identity;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Drivers.TopUpWallet;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class TopUpWalletCommandHandlerTest : IAsyncLifetime
{
    private readonly WebAppFactory _factory;
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly TestCurrentUser _currentUser;

    public TopUpWalletCommandHandlerTest(WebAppFactory factory)
    {
        _factory = factory;
        _mediator = factory.CreateMediator();
        _context = factory.CreateAppDbContext();
        _currentUser = factory.TestUser;
    }

    public Task InitializeAsync() => _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Handle_ShouldIncreaseBalanceAndCreateTransaction_WhenAgent()
    {
        var driver = await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: true);
        _currentUser.Returns(TestUsers.Agent, Roles.Agent);

        var result = await _mediator.Send(new TopUpWalletCommand(150m, driver.Id), CancellationToken.None);

        Assert.True(result.IsSuccess, result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");

        var wallet = await _context.TbDriverWallets.AsNoTracking().FirstAsync(w => w.DriverId == driver.Id);
        Assert.Equal(150m, wallet.Balance);

        var transactions = await _context.TbDriverWalletTransactions.AsNoTracking()
            .Where(t => t.DriverWalletId == wallet.Id).ToListAsync();

        Assert.Single(transactions);
        Assert.Equal(150m, transactions[0].Amount);
    }

    [Fact]
    public async Task Handle_ShouldAccumulateBalance_WhenToppedUpTwice()
    {
        var driver = await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: true);
        _currentUser.Returns(TestUsers.Agent, Roles.Agent);

        await _mediator.Send(new TopUpWalletCommand(100m, driver.Id), CancellationToken.None);
        var result = await _mediator.Send(new TopUpWalletCommand(50m, driver.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);

        var wallet = await _context.TbDriverWallets.AsNoTracking().FirstAsync(w => w.DriverId == driver.Id);
        Assert.Equal(150m, wallet.Balance);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenNotAgent()
    {
        var driver = await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: true);
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var result = await _mediator.Send(new TopUpWalletCommand(100m, driver.Id), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);

        var wallet = await _context.TbDriverWallets.AsNoTracking().FirstAsync(w => w.DriverId == driver.Id);
        Assert.Equal(0m, wallet.Balance);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenUnauthenticated()
    {
        var result = await _mediator.Send(new TopUpWalletCommand(100m, Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnDriverNotFound_WhenDriverMissing()
    {
        _currentUser.Returns(TestUsers.Agent, Roles.Agent);

        var result = await _mediator.Send(new TopUpWalletCommand(100m, Guid.NewGuid()), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.DriverNotFound.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnDriverNotFound_WhenDriverNotApproved()
    {
        var driver = await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: false);
        _currentUser.Returns(TestUsers.Agent, Roles.Agent);

        var result = await _mediator.Send(new TopUpWalletCommand(100m, driver.Id), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.DriverNotFound.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnWalletNotFound_WhenDriverHasNoWallet()
    {
        var driver = await DriverTestData.SeedDriverAsync(
            _context, TestUsers.Driver1.Id, approved: true, withWallet: false);
        _currentUser.Returns(TestUsers.Agent, Roles.Agent);

        var result = await _mediator.Send(new TopUpWalletCommand(100m, driver.Id), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.WalletNotFound.Code, result.TopError.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task Handle_ShouldReturnValidationError_WhenAmountNotPositive(double amount)
    {
        var driver = await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: true);
        _currentUser.Returns(TestUsers.Agent, Roles.Agent);

        var result = await _mediator.Send(new TopUpWalletCommand((decimal)amount, driver.Id), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal("Amount", result.TopError.Code);

        var wallet = await _context.TbDriverWallets.AsNoTracking().FirstAsync(w => w.DriverId == driver.Id);
        Assert.Equal(0m, wallet.Balance);
    }
}
