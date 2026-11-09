using MediatR;
using Microsoft.Extensions.DependencyInjection;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;
using UberSy.Test.Common.Security;
using Xunit;

namespace UberSy.Api.IntegrationTests.Common;

/// <summary>
/// Shared plumbing for subcutaneous handler tests:
/// resets the database before every test and exposes mediator, db context and the fake current user.
/// Derived classes must still carry [Collection(WebAppFactoryCollection.CollectionName)].
/// </summary>
public abstract class AppTestBase : IAsyncLifetime
{
    protected readonly WebAppFactory Factory;
    protected readonly IMediator Mediator;
    protected readonly IApplicationDbContext Context;
    protected readonly TestCurrentUser CurrentUser;

    protected AppTestBase(WebAppFactory factory)
    {
        Factory = factory;
        Mediator = factory.CreateMediator();
        Context = factory.CreateAppDbContext();
        CurrentUser = factory.TestUser;
    }

    public Task InitializeAsync() => Factory.ResetDatabaseAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    // ---------- current user helpers ----------

    // The manager is identified by id (SystemConstants.ManagerId), not by role
    protected void AsManager() => CurrentUser.Returns(TestUsers.Admin, Roles.Admin);

    protected void AsOwner1() => CurrentUser.Returns(TestUsers.Owner1, Roles.Owner);

    protected void AsOwner2() => CurrentUser.Returns(TestUsers.Owner2, Roles.Owner);

    protected void AsCustomer() => CurrentUser.Returns(TestUsers.Customer, Roles.Customer);

    protected void AsCustomer2() => CurrentUser.Returns(TestUsers.Customer2, Roles.Customer);

    protected void AsDriver1() => CurrentUser.Returns(TestUsers.Driver1, Roles.Driver);

    protected void AsDriver2() => CurrentUser.Returns(TestUsers.Driver2, Roles.Driver);

    protected void AsDriver3() => CurrentUser.Returns(TestUsers.Driver3, Roles.Driver);

    protected void AsAnonymous() => CurrentUser.Reset();

    // ---------- assertion helpers ----------

    protected static string Describe<T>(Result<T> result)
        => result.IsError ? $"{result.TopError.Code}: {result.TopError.Description}" : string.Empty;

    protected static void AssertSuccess<T>(Result<T> result)
        => Assert.True(result.IsSuccess, Describe(result));

    protected static void AssertError<T>(Result<T> result, Error expected)
    {
        Assert.True(result.IsError, "Expected an error but the result was successful.");
        Assert.Equal(expected.Code, result.TopError.Code);
    }

    protected static void AssertErrorCode<T>(Result<T> result, string expectedCode)
    {
        Assert.True(result.IsError, "Expected an error but the result was successful.");
        Assert.Equal(expectedCode, result.TopError.Code);
    }

    // ---------- identity helpers (roles survive Respawn, so tests must clean up after themselves) ----------

    protected async Task<bool> UserHasRoleAsync(Guid userId, string role)
    {
        using var scope = Factory.Services.CreateScope();
        var roles = scope.ServiceProvider.GetRequiredService<IUserRoleService>();

        var result = await roles.GetUserRolesAsync(userId);
        return result.IsSuccess && result.Value.Roles is not null && result.Value.Roles.Contains(role);
    }

    protected async Task RemoveRoleIfAssignedAsync(Guid userId, string role)
    {
        using var scope = Factory.Services.CreateScope();
        var roles = scope.ServiceProvider.GetRequiredService<IUserRoleService>();

        await roles.RemoveFromRoleAsync(userId, role);
    }
}
