using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Drivers.Commands.UpdateDriverLocation;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Drivers.UpdateDriverLocation;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class UpdateDriverLocationCommandHandlerTest : IAsyncLifetime
{
    private readonly WebAppFactory _factory;
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;

    public UpdateDriverLocationCommandHandlerTest(WebAppFactory factory)
    {
        _factory = factory;
        _mediator = factory.CreateMediator();
        _context = factory.CreateAppDbContext();
    }

    public Task InitializeAsync() => _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Handle_ShouldCreateLocation_WhenFirstUpdate()
    {
        var driver = await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: true, online: true);

        var result = await _mediator.Send(
            new UpdateDriverLocationCommand(driver.Id, 33.5138, 36.2765), CancellationToken.None);

        Assert.True(result.IsSuccess, result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");

        var location = await _context.TbDriverLocations.AsNoTracking().SingleAsync(l => l.DriverId == driver.Id);
        Assert.Equal(36.2765, location.Boundary.X, 4); // X = longitude
        Assert.Equal(33.5138, location.Boundary.Y, 4); // Y = latitude
    }

    [Fact]
    public async Task Handle_ShouldUpdateExistingLocation_WhenCalledTwice()
    {
        var driver = await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: true, online: true);

        await _mediator.Send(new UpdateDriverLocationCommand(driver.Id, 33.5138, 36.2765), CancellationToken.None);
        var result = await _mediator.Send(new UpdateDriverLocationCommand(driver.Id, 36.2021, 37.1343), CancellationToken.None);

        Assert.True(result.IsSuccess, result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");

        var locations = await _context.TbDriverLocations.AsNoTracking()
            .Where(l => l.DriverId == driver.Id).ToListAsync();

        Assert.Single(locations); // updated in place, no duplicate row
        Assert.Equal(37.1343, locations[0].Boundary.X, 4);
        Assert.Equal(36.2021, locations[0].Boundary.Y, 4);
    }

    [Fact]
    public async Task Handle_ShouldReturnDriverNotFound_WhenDriverMissing()
    {
        var result = await _mediator.Send(
            new UpdateDriverLocationCommand(Guid.NewGuid(), 33.5, 36.2), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.DriverNotFound.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnDriverOffline_WhenDriverNotOnline()
    {
        var driver = await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: true, online: false);

        var result = await _mediator.Send(
            new UpdateDriverLocationCommand(driver.Id, 33.5, 36.2), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.DriverOffline.Code, result.TopError.Code);
        Assert.False(await _context.TbDriverLocations.AsNoTracking().AnyAsync());
    }

    [Theory]
    [InlineData(91, 36.2)]
    [InlineData(33.5, 181)]
    public async Task Handle_ShouldReturnValidationError_WhenCoordinatesOutOfRange(double lat, double lon)
    {
        var driver = await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: true, online: true);

        var result = await _mediator.Send(
            new UpdateDriverLocationCommand(driver.Id, lat, lon), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Contains(result.TopError.Code, new[] { "Latitude", "Longitude" });
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenDriverHasActiveOrder()
    {
        var driver = await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: true, online: true);
        await DriverTestData.SeedFoodOrderAsync(_context, driver.Id, ServiceRequestStatus.OutForDelivery);

        var result = await _mediator.Send(
            new UpdateDriverLocationCommand(driver.Id, 33.5138, 36.2765), CancellationToken.None);

        Assert.True(result.IsSuccess, result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");
        Assert.True(await _context.TbDriverLocations.AsNoTracking().AnyAsync(l => l.DriverId == driver.Id));
    }
}
