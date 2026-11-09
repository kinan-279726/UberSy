using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Drivers.Commands.RegisterDriver;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Identity;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Drivers.RegisterDriver;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class RegisterDriverCommandHandlerTest : IAsyncLifetime
{
    private readonly WebAppFactory _factory;
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly TestCurrentUser _currentUser;

    public RegisterDriverCommandHandlerTest(WebAppFactory factory)
    {
        _factory = factory;
        _mediator = factory.CreateMediator();
        _context = factory.CreateAppDbContext();
        _currentUser = factory.TestUser;
    }

    public Task InitializeAsync() => _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    private static RegisterDriverCommand ValidCommand(string nationalId = "123456789")
        => new(VehicleType.Motorcycle, null, nationalId, null, "LIC-1", "PLATE-1", "Honda", "Red");

    [Fact]
    public async Task Handle_ShouldCreateDriverAndWallet_WhenValid()
    {
        _currentUser.Returns(TestUsers.Customer, Roles.Customer);

        var result = await _mediator.Send(ValidCommand(), CancellationToken.None);

        Assert.True(result.IsSuccess, result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");

        var driver = await _context.TbDrivers.AsNoTracking()
            .FirstOrDefaultAsync(d => d.UserId == TestUsers.Customer.Id);

        Assert.NotNull(driver);
        Assert.False(driver!.IsApproved);
        Assert.Equal(ServiceTier.Standard, driver.ServiceTier);
        Assert.Equal("123456789", driver.NationalId);

        var wallet = await _context.TbDriverWallets.AsNoTracking()
            .FirstOrDefaultAsync(w => w.DriverId == driver.Id);

        Assert.NotNull(wallet);
        Assert.Equal(0m, wallet!.Balance);
    }

    [Fact]
    public async Task Handle_ShouldTrimNationalId()
    {
        _currentUser.Returns(TestUsers.Customer, Roles.Customer);

        var result = await _mediator.Send(ValidCommand("  987654321  "), CancellationToken.None);

        Assert.True(result.IsSuccess, result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");

        var driver = await _context.TbDrivers.AsNoTracking()
            .FirstAsync(d => d.UserId == TestUsers.Customer.Id);
        Assert.Equal("987654321", driver.NationalId);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenUnauthenticated()
    {
        var result = await _mediator.Send(ValidCommand(), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.False(await _context.TbDrivers.AsNoTracking().AnyAsync());
    }

    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenUserAlreadyRegistered()
    {
        await DriverTestData.SeedDriverAsync(_context, TestUsers.Customer.Id);
        _currentUser.Returns(TestUsers.Customer, Roles.Customer);

        var result = await _mediator.Send(ValidCommand("555555555"), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.DriverAlreadyRegistered.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnConflict_WhenNationalIdAlreadyUsed()
    {
        await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver2.Id, nationalId: "123456789");
        _currentUser.Returns(TestUsers.Customer, Roles.Customer);

        var result = await _mediator.Send(ValidCommand("123456789"), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.NationalIdAlreadyRegistered.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnDomainError_WhenMotorcycleMissingLicense()
    {
        _currentUser.Returns(TestUsers.Customer, Roles.Customer);

        var command = new RegisterDriverCommand(VehicleType.Motorcycle, null, "111111111", null, null, null, null, null);
        var result = await _mediator.Send(command, CancellationToken.None);

        Assert.True(result.IsError);
        Assert.False(await _context.TbDrivers.AsNoTracking().AnyAsync());
        Assert.False(await _context.TbDriverWallets.AsNoTracking().AnyAsync());
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenNationalIdEmpty()
    {
        _currentUser.Returns(TestUsers.Customer, Roles.Customer);

        var result = await _mediator.Send(ValidCommand(""), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal("NationalId", result.TopError.Code);
    }
}
