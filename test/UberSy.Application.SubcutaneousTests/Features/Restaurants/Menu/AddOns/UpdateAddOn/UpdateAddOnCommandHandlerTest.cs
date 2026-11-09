using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants.AddOns;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.AddOns.UpdateAddOn;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class UpdateAddOnCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldUpdateAddOn_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);
        AsOwner1();

        var result = await Mediator.Send(
            AddOnCommandFactory.CreateUpdateAddOnCommand(addOn.Id, "  صوص ثوم  ", "Garlic Sauce", 1.5m),
            CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbAddOns.AsNoTracking().FirstAsync(a => a.Id == addOn.Id);
        Assert.Equal("صوص ثوم", saved.NameAr); // trimmed by the domain
        Assert.Equal("Garlic Sauce", saved.NameEn);
        Assert.Equal(1.5m, saved.Price);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);
        AsOwner2();

        var result = await Mediator.Send(
            AddOnCommandFactory.CreateUpdateAddOnCommand(addOn.Id, price: 99m), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);

        var saved = await Context.TbAddOns.AsNoTracking().FirstAsync(a => a.Id == addOn.Id);
        Assert.Equal(2.5m, saved.Price);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);

        var result = await Mediator.Send(AddOnCommandFactory.CreateUpdateAddOnCommand(addOn.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnAddOnNotFound_WhenMissing()
    {
        // KNOWN BUG: the handler returns ApplicationErrors.ItemNotFound instead of AddOnNotFound
        AsOwner1();

        var result = await Mediator.Send(AddOnCommandFactory.CreateUpdateAddOnCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.AddOnNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenPriceNotPositive()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);
        AsOwner1();

        var result = await Mediator.Send(
            AddOnCommandFactory.CreateUpdateAddOnCommand(addOn.Id, price: 0m), CancellationToken.None);

        AssertErrorCode(result, "Price");
    }

    [Fact]
    public async Task Handle_ShouldReturnDomainError_WhenNameTooShort()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);
        AsOwner1();

        var result = await Mediator.Send(
            AddOnCommandFactory.CreateUpdateAddOnCommand(addOn.Id, nameEn: "A"), CancellationToken.None);

        AssertError(result, AddOnErrors.NameEnTooShort);
    }
}
