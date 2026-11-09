using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Drivers.Commands.GoOnline;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Drivers;
using UberSy.Domain.Drivers.Enums;
using UberSy.Domain.Identity;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Drivers.GoOnline;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GoOnlineCommandHandlerTest : IAsyncLifetime
{
    private readonly WebAppFactory _factory;
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    private readonly TestCurrentUser _currentUser;

    public GoOnlineCommandHandlerTest(WebAppFactory factory)
    {
        _factory = factory;
        _mediator = factory.CreateMediator();
        _context = factory.CreateAppDbContext();
        _currentUser = factory.TestUser;
    }

    public Task InitializeAsync() => _factory.ResetDatabaseAsync();
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Handle_ShouldSetOnline_WhenApprovedDriver()
    {
        var driver = await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: true);
        _currentUser.Returns(TestUsers.Driver1, Roles.Driver);

        var result = await _mediator.Send(new GoOnlineCommand(), CancellationToken.None);

        Assert.True(result.IsSuccess, result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : "");

        var saved = await _context.TbDrivers.AsNoTracking().FirstAsync(d => d.Id == driver.Id);
        Assert.Equal(DriverStatus.Online, saved.Status);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenNotDriverRole()
    {
        _currentUser.Returns(TestUsers.Customer, Roles.Customer);

        var result = await _mediator.Send(new GoOnlineCommand(), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenUnauthenticated()
    {
        var result = await _mediator.Send(new GoOnlineCommand(), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.AccessDenied.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnDriverNotFound_WhenNoDriverProfile()
    {
        _currentUser.Returns(TestUsers.Driver1, Roles.Driver);

        var result = await _mediator.Send(new GoOnlineCommand(), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.DriverNotFound.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotApproved_WhenDriverNotApproved()
    {
        await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: false);
        _currentUser.Returns(TestUsers.Driver1, Roles.Driver);

        var result = await _mediator.Send(new GoOnlineCommand(), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(ApplicationErrors.DriverNotApproved.Code, result.TopError.Code);
    }

    [Fact]
    public async Task Handle_ShouldReturnDomainError_WhenAlreadyOnline()
    {
        await DriverTestData.SeedDriverAsync(_context, TestUsers.Driver1.Id, approved: true, online: true);
        _currentUser.Returns(TestUsers.Driver1, Roles.Driver);

        var result = await _mediator.Send(new GoOnlineCommand(), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.Equal(DriverErrors.DriverAlreadyOnline.Code, result.TopError.Code);
    }
}
