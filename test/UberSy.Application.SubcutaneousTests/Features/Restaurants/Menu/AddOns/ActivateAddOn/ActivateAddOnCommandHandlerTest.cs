using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants.AddOns;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.AddOns.ActivateAddOn;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class ActivateAddOnCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldActivateAddOn_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: false);
        AsOwner1();

        var result = await Mediator.Send(AddOnCommandFactory.CreateActivateAddOnCommand(addOn.Id), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbAddOns.AsNoTracking().FirstAsync(a => a.Id == addOn.Id);
        Assert.True(saved.IsActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: false);
        AsOwner2();

        var result = await Mediator.Send(AddOnCommandFactory.CreateActivateAddOnCommand(addOn.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);

        var saved = await Context.TbAddOns.AsNoTracking().FirstAsync(a => a.Id == addOn.Id);
        Assert.False(saved.IsActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: false);

        var result = await Mediator.Send(AddOnCommandFactory.CreateActivateAddOnCommand(addOn.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnAddOnNotFound_WhenMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(AddOnCommandFactory.CreateActivateAddOnCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.AddOnNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyActive_WhenAddOnActive()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: true);
        AsOwner1();

        var result = await Mediator.Send(AddOnCommandFactory.CreateActivateAddOnCommand(addOn.Id), CancellationToken.None);

        AssertError(result, AddOnErrors.AddOnAlreadyActive);
    }
}
