using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants.Items;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Items.DeactivateItem;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class DeactivateItemCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldDeactivateItem_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        AsOwner1();

        var result = await Mediator.Send(ItemCommandFactory.CreateDeactivateItemCommand(item.Id), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbItems.AsNoTracking().FirstAsync(i => i.Id == item.Id);
        Assert.False(saved.IsActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        AsOwner2();

        var result = await Mediator.Send(ItemCommandFactory.CreateDeactivateItemCommand(item.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);

        var saved = await Context.TbItems.AsNoTracking().FirstAsync(i => i.Id == item.Id);
        Assert.True(saved.IsActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);

        var result = await Mediator.Send(ItemCommandFactory.CreateDeactivateItemCommand(item.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnItemNotFound_WhenMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(ItemCommandFactory.CreateDeactivateItemCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.ItemNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyInactive_WhenItemInactive()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id, active: false);
        AsOwner1();

        var result = await Mediator.Send(ItemCommandFactory.CreateDeactivateItemCommand(item.Id), CancellationToken.None);

        AssertError(result, ItemErrors.ItemAlreadyInactive);
    }
}
