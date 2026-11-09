using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.UpdateOperatingHours;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class UpdateOperatingHoursCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldUpdateHours_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateOperatingHoursCommand(
                restaurant.Id, new TimeOnly(8, 30), new TimeOnly(22, 0)),
            CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.Equal(new TimeOnly(8, 30), saved.OpeningTime);
        Assert.Equal(new TimeOnly(22, 0), saved.ClosingTime);
    }

    [Fact]
    public async Task Handle_ShouldUpdateHours_WhenManager()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsManager();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateOperatingHoursCommand(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner2();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateOperatingHoursCommand(
                restaurant.Id, new TimeOnly(1, 0), new TimeOnly(2, 0)),
            CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.Equal(new TimeOnly(9, 0), saved.OpeningTime); // factory default
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateOperatingHoursCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenRestaurantMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateOperatingHoursCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenClosingNotAfterOpening()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateOperatingHoursCommand(
                restaurant.Id, new TimeOnly(20, 0), new TimeOnly(8, 0)),
            CancellationToken.None);

        AssertErrorCode(result, "ClosingTime");
    }
}
