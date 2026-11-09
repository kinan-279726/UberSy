using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.RejectRestaurant;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class RejectRestaurantCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldRejectRestaurant_WhenManager()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: true);
        AsManager();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateRejectRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.False(saved.IsApproved);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenNotManager()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: true);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateRejectRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.True(saved.IsApproved);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenUnauthenticated()
    {
        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateRejectRestaurantCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenRestaurantMissing()
    {
        AsManager();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateRejectRestaurantCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyRejected_WhenRestaurantNotApproved()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: false);
        AsManager();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateRejectRestaurantCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, RestaurantErrors.RestaurantAlreadyRejected);
    }
}
