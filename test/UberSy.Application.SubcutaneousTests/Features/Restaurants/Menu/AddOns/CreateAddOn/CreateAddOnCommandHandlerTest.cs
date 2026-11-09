using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants.AddOns;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.AddOns.CreateAddOn;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class CreateAddOnCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldCreateActiveAddOn_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            AddOnCommandFactory.CreateCreateAddOnCommand(restaurant.Id, "جبنة", "Cheese", 3.25m), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(restaurant.Id, result.Value.RestaurantId);
        Assert.True(result.Value.IsActive);

        var saved = await Context.TbAddOns.AsNoTracking().FirstAsync(a => a.Id == result.Value.Id);
        Assert.Equal("جبنة", saved.NameAr);
        Assert.Equal("Cheese", saved.NameEn);
        Assert.Equal(3.25m, saved.Price);
        Assert.True(saved.IsActive);
    }

    [Fact]
    public async Task Handle_ShouldCreateAddOn_WhenManager()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsManager();

        var result = await Mediator.Send(
            AddOnCommandFactory.CreateCreateAddOnCommand(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner2();

        var result = await Mediator.Send(
            AddOnCommandFactory.CreateCreateAddOnCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);
        Assert.False(await Context.TbAddOns.AsNoTracking().AnyAsync());
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);

        var result = await Mediator.Send(
            AddOnCommandFactory.CreateCreateAddOnCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenRestaurantMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(
            AddOnCommandFactory.CreateCreateAddOnCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenPriceNotPositive()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            AddOnCommandFactory.CreateCreateAddOnCommand(restaurant.Id, price: 0m), CancellationToken.None);

        AssertErrorCode(result, "Price");
    }

    [Fact]
    public async Task Handle_ShouldReturnDomainError_WhenNameTooShort()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            AddOnCommandFactory.CreateCreateAddOnCommand(restaurant.Id, nameEn: "A"), CancellationToken.None);

        AssertError(result, AddOnErrors.NameEnTooShort);
    }
}
