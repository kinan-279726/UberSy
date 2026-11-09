using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.UpdateCommissionRate;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class UpdateCommissionRateCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldUpdateRate_WhenManager()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsManager();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateCommissionRateCommand(restaurant.Id, 12.5m), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.Equal(12.5m, saved.CommissionRate);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenOwnerTriesToChangeOwnRate()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateCommissionRateCommand(restaurant.Id, 1m), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.Equal(10m, saved.CommissionRate); // factory default
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateCommissionRateCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenNotManagerAndRestaurantMissing()
    {
        // The permission check should come before the lookup so that non-managers cannot probe for restaurant ids.
        // KNOWN ISSUE: the handler looks the restaurant up first and returns Restaurant_NotFound.
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateCommissionRateCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenManagerAndRestaurantMissing()
    {
        AsManager();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateCommissionRateCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task Handle_ShouldReturnValidationError_WhenRateOutOfRange(double rate)
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsManager();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateCommissionRateCommand(restaurant.Id, (decimal)rate), CancellationToken.None);

        AssertErrorCode(result, "CommissionRate");
    }
}
