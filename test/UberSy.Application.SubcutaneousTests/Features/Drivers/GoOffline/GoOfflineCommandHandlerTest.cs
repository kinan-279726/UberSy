using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Drivers.Commands.GoOffline;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Drivers;
using UberSy.Domain.Drivers.Enums;
using UberSy.Domain.Identity;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Drivers.GoOffline;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GoOfflineCommandHandlerTest : IAsyncLifetime
{
    private readonly WebAppFactory _factory;
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly TestCurrentUser _currentUser;

    public GoOfflineCommandHandlerTest(WebAppFactory factory)
    {
        _factory = factory;
        _mediator = factory.CreateMediator();
        _context = factory.CreateAppDbContext();
        _currentUser = factory.TestUser;
    }

    public Task InitializeAsync() => _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Handle_ShouldSetOffline_WhenOnlineDriverHasNoActiveOrder()
    {
        var driver = await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: true, online: true);
        _currentUser.Returns(TestUsers.Driver1, Roles.Driver);

        var result = await _mediator.Send(new GoOfflineCommand(), CancellationToken.None);

        Assert.True(result.IsSuccess, result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");

        var saved = await _context.TbDrivers.AsNoTracking().FirstAsync(d => d.Id == driver.Id);
        Assert.Equal(DriverStatus.Offline, saved.Status);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenNotDriverRole()
    {
        _currentUser.Returns(TestUsers.Customer, Roles.Customer);

        var result = await _mediator.Send(new GoOfflineCommand(), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenUnauthenticated()
    {
        var result = await _mediator.Send(new GoOfflineCommand(), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnDriverNotFound_WhenNoDriverProfile()
    {
        _currentUser.Returns(TestUsers.Driver1, Roles.Driver);

        var result = await _mediator.Send(new GoOfflineCommand(), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.DriverNotFound.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotApproved_WhenDriverNotApproved()
    {
        await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: false);
        _currentUser.Returns(TestUsers.Driver1, Roles.Driver);

        var result = await _mediator.Send(new GoOfflineCommand(), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.DriverNotApproved.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnDomainError_WhenAlreadyOffline()
    {
        await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: true, online: false);
        _currentUser.Returns(TestUsers.Driver1, Roles.Driver);

        var result = await _mediator.Send(new GoOfflineCommand(), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(DriverErrors.DriverAlreadyOffline.Code, result.TopError.Code);
    }

    [Theory]
    [InlineData(ServiceRequestStatus.Preparing)]
    [InlineData(ServiceRequestStatus.OutForDelivery)]
    public async Task Handle_ShouldReturnAlreadyOnDelivery_WhenDriverHasActiveOrder(ServiceRequestStatus status)
    {
        var driver = await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: true, online: true);
        await DriverTestData.SeedFoodOrderAsync(_context, driver.Id, status);
        _currentUser.Returns(TestUsers.Driver1, Roles.Driver);

        var result = await _mediator.Send(new GoOfflineCommand(), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.DriverAlreadyOnDelivery.Code, result.TopError.Code);

        var saved = await _context.TbDrivers.AsNoTracking().FirstAsync(d => d.Id == driver.Id);
        Assert.Equal(DriverStatus.Online, saved.Status);
    }

    [Theory]
    [InlineData(ServiceRequestStatus.Delivered)]
    [InlineData(ServiceRequestStatus.Cancelled)]
    public async Task Handle_ShouldSucceed_WhenOnlyFinishedOrdersExist(ServiceRequestStatus status)
    {
        var driver = await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: true, online: true);
        await DriverTestData.SeedFoodOrderAsync(_context, driver.Id, status);
        _currentUser.Returns(TestUsers.Driver1, Roles.Driver);

        var result = await _mediator.Send(new GoOfflineCommand(), CancellationToken.None);

        Assert.True(result.IsSuccess, result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");

        var saved = await _context.TbDrivers.AsNoTracking().FirstAsync(d => d.Id == driver.Id);
        Assert.Equal(DriverStatus.Offline, saved.Status);
    }
}
