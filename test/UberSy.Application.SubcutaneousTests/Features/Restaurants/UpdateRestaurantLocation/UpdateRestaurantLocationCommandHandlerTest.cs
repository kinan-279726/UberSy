using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.UpdateRestaurantLocation;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class UpdateRestaurantLocationCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldUpdateLocation_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantLocationCommand(restaurant.Id, 36.2021, 37.1343),
            CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.Equal(37.1343, saved.Boundary.X, 4); // X = longitude
        Assert.Equal(36.2021, saved.Boundary.Y, 4); // Y = latitude
        Assert.Equal(4326, saved.Boundary.SRID);
    }

    [Fact]
    public async Task Handle_ShouldUpdateLocation_WhenManager()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsManager();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantLocationCommand(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner2();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantLocationCommand(restaurant.Id, 36.2021, 37.1343),
            CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.Equal(36.2765, saved.Boundary.X, 4); // unchanged (factory default longitude)
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantLocationCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenRestaurantMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantLocationCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Theory]
    [InlineData(91, 36.2)]
    [InlineData(-91, 36.2)]
    [InlineData(33.5, 181)]
    [InlineData(33.5, -181)]
    public async Task Handle_ShouldRejectOutOfRangeCoordinates(double latitude, double longitude)
    {
        // KNOWN GAP: there is no validator for this command and NetTopologySuite does not check lat/lon ranges,
        // so these invalid coordinates are currently accepted. Add a validator like UpdateDriverLocationCommandValidator.
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantLocationCommand(restaurant.Id, latitude, longitude),
            CancellationToken.None);

        Assert.True(result.IsError, "Out-of-range coordinates should be rejected.");
    }
}
