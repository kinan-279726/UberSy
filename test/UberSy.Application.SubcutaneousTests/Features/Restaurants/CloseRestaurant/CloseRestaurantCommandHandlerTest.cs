using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.CloseRestaurant;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class CloseRestaurantCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldCloseRestaurant_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, open: true);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateCloseRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.False(saved.IsOpenNow);
    }

    [Fact]
    public async Task Handle_ShouldCloseRestaurant_WhenManager()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, open: true);
        AsManager();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateCloseRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, open: true);
        AsOwner2();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateCloseRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.True(saved.IsOpenNow);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, open: true);

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateCloseRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenRestaurantMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateCloseRestaurantCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyClosed_WhenRestaurantClosed()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: true);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateCloseRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, RestaurantErrors.RestaurantAlreadyClosed);
    }
}
