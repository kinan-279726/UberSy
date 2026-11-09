using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Drivers.Commands.ApproveDriver;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Identity;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Drivers.ApproveDriver;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class ApproveDriverCommandHandlerTest : IAsyncLifetime
{
    private readonly WebAppFactory _factory;
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly TestCurrentUser _currentUser;

    public ApproveDriverCommandHandlerTest(WebAppFactory factory)
    {
        _factory = factory;
        _mediator = factory.CreateMediator();
        _context = factory.CreateAppDbContext();
        _currentUser = factory.TestUser;
    }

    public Task InitializeAsync() => _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Handle_ShouldApproveDriver_WhenAdmin()
    {
        var driver = await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: false);
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var result = await _mediator.Send(
            new ApproveDriverCommand(driver.Id, ServiceTier.Standard), CancellationToken.None);

        Assert.True(result.IsSuccess, result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");

        var saved = await _context.TbDrivers.AsNoTracking().FirstAsync(d => d.Id == driver.Id);
        Assert.True(saved.IsApproved);
    }

    [Fact]
    public async Task Handle_ShouldBeIdempotent_WhenAlreadyApproved()
    {
        var driver = await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: false);
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var first = await _mediator.Send(new ApproveDriverCommand(driver.Id, ServiceTier.Standard), CancellationToken.None);
        var second = await _mediator.Send(new ApproveDriverCommand(driver.Id, ServiceTier.Standard), CancellationToken.None);

        Assert.True(first.IsSuccess);
        Assert.True(second.IsSuccess, second.IsError ? $"{second.TopError.Code}: {second.TopError.Description}" : "");
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenNotAdmin()
    {
        var driver = await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: false);
        _currentUser.Returns(TestUsers.Customer, Roles.Customer);

        var result = await _mediator.Send(
            new ApproveDriverCommand(driver.Id, ServiceTier.Standard), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);

        var saved = await _context.TbDrivers.AsNoTracking().FirstAsync(d => d.Id == driver.Id);
        Assert.False(saved.IsApproved);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenUnauthenticated()
    {
        var result = await _mediator.Send(
            new ApproveDriverCommand(Guid.NewGuid(), ServiceTier.Standard), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnDriverNotFound_WhenDriverMissing()
    {
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var result = await _mediator.Send(
            new ApproveDriverCommand(Guid.NewGuid(), ServiceTier.Standard), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.DriverNotFound.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenDriverIdEmpty()
    {
        _currentUser.Returns(TestUsers.Admin, Roles.Admin);

        var result = await _mediator.Send(
            new ApproveDriverCommand(Guid.Empty, ServiceTier.Standard), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal("DriverId", result.TopError.Code);
    }
}
