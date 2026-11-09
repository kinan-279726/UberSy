using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.UpdateRestaurantImages;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class UpdateRestaurantImagesCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldUpdateImages_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantImagesCommand(
                restaurant.Id, "https://cdn.ubersy.com/logo.png", "https://cdn.ubersy.com/cover.png"),
            CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.Equal("https://cdn.ubersy.com/logo.png", saved.LogoUrl);
        Assert.Equal("https://cdn.ubersy.com/cover.png", saved.CoverUrl);
    }

    [Fact]
    public async Task Handle_ShouldClearImages_WhenNullsProvided()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantImagesCommand(restaurant.Id, "https://x/logo.png", "https://x/cover.png"),
            CancellationToken.None);

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantImagesCommand(restaurant.Id, null, null), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.Null(saved.LogoUrl);
        Assert.Null(saved.CoverUrl);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner2();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantImagesCommand(restaurant.Id, "https://x/logo.png", null),
            CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.Null(saved.LogoUrl);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantImagesCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenRestaurantMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantImagesCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnDomainError_WhenLogoUrlTooLong()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantImagesCommand(restaurant.Id, new string('a', 1001), null),
            CancellationToken.None);

        AssertError(result, RestaurantErrors.LogoUrlTooLong);
    }

    [Fact]
    public async Task Handle_ShouldReturnDomainError_WhenCoverUrlTooLong()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantImagesCommand(restaurant.Id, null, new string('a', 1001)),
            CancellationToken.None);

        AssertError(result, RestaurantErrors.CoverUrlTooLong);
    }
}
