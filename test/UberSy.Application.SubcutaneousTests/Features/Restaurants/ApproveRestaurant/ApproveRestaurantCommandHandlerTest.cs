using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Identity;
using UberSy.Domain.Restaurants;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.ApproveRestaurant;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class ApproveRestaurantCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldApproveRestaurantAndAssignOwnerRole_WhenManager()
    {
        // AspNetUserRoles is ignored by Respawn, so make sure the role is not left over from another test
        await RemoveRoleIfAssignedAsync(TestUsers.Owner1.Id, Roles.Owner);
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsManager();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateApproveRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.True(saved.IsApproved);
        Assert.True(await UserHasRoleAsync(TestUsers.Owner1.Id, Roles.Owner));

        await RemoveRoleIfAssignedAsync(TestUsers.Owner1.Id, Roles.Owner);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenNotManager()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateApproveRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.False(saved.IsApproved);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateApproveRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenRestaurantMissing()
    {
        AsManager();
        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateApproveRestaurantCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyApproved_WhenRestaurantApproved()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: true);
        AsManager();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateApproveRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, RestaurantErrors.RestaurantAlreadyApproved);
    }
}
