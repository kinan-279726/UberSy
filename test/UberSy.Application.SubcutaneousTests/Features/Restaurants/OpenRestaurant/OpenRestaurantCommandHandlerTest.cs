using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.OpenRestaurant;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class OpenRestaurantCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldOpenRestaurant_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: true);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateOpenRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.True(saved.IsOpenNow);
    }

    [Fact]
    public async Task Handle_ShouldOpenRestaurant_WhenManager()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: true);
        AsManager();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateOpenRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: true);
        AsOwner2();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateOpenRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.False(saved.IsOpenNow);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: true);

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateOpenRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenRestaurantMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateOpenRestaurantCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotApproved_WhenRestaurantNotApproved()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: false);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateOpenRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, RestaurantErrors.RestaurantNotApproved);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyOpen_WhenRestaurantOpen()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, open: true);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateOpenRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, RestaurantErrors.RestaurantAlreadyOpen);
    }
}
