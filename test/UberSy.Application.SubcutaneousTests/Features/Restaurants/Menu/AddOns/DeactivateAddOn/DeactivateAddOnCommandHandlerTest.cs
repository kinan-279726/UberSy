using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants.AddOns;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.AddOns.DeactivateAddOn;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class DeactivateAddOnCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldDeactivateAddOn_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: true);
        AsOwner1();

        var result = await Mediator.Send(AddOnCommandFactory.CreateDeactivateAddOnCommand(addOn.Id), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbAddOns.AsNoTracking().FirstAsync(a => a.Id == addOn.Id);
        Assert.False(saved.IsActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: true);
        AsOwner2();

        var result = await Mediator.Send(AddOnCommandFactory.CreateDeactivateAddOnCommand(addOn.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);

        var saved = await Context.TbAddOns.AsNoTracking().FirstAsync(a => a.Id == addOn.Id);
        Assert.True(saved.IsActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: true);

        var result = await Mediator.Send(AddOnCommandFactory.CreateDeactivateAddOnCommand(addOn.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnAddOnNotFound_WhenMissing()
    {
        // KNOWN BUG: the handler returns ApplicationErrors.ItemNotFound instead of AddOnNotFound
        AsOwner1();

        var result = await Mediator.Send(AddOnCommandFactory.CreateDeactivateAddOnCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.AddOnNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyInactive_WhenAddOnInactive()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: false);
        AsOwner1();

        var result = await Mediator.Send(AddOnCommandFactory.CreateDeactivateAddOnCommand(addOn.Id), CancellationToken.None);

        AssertError(result, AddOnErrors.AddOnAlreadyInactive);
    }
}
