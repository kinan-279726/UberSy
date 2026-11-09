using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Items.MoveItemToCategory;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class MoveItemToCategoryCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldMoveItem_WhenCategoryInSameRestaurant()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var from = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var to = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, from.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateMoveItemToCategoryCommand(item.Id, to.Id), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbItems.AsNoTracking().FirstAsync(i => i.Id == item.Id);
        Assert.Equal(to.Id, saved.CategoryId);
    }

    [Fact]
    public async Task Handle_ShouldReturnCategoryNotFound_WhenCategoryBelongsToAnotherRestaurant()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var otherRestaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner2.Id);
        var from = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var foreign = await RestaurantTestData.SeedCategoryAsync(Context, otherRestaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, from.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateMoveItemToCategoryCommand(item.Id, foreign.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.CategoryNotFound);

        var saved = await Context.TbItems.AsNoTracking().FirstAsync(i => i.Id == item.Id);
        Assert.Equal(from.Id, saved.CategoryId);
    }

    [Fact]
    public async Task Handle_ShouldReturnCategoryNotFound_WhenCategoryMissing()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var from = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, from.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateMoveItemToCategoryCommand(item.Id, Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.CategoryNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var from = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var to = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, from.Id);
        AsOwner2();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateMoveItemToCategoryCommand(item.Id, to.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);

        var saved = await Context.TbItems.AsNoTracking().FirstAsync(i => i.Id == item.Id);
        Assert.Equal(from.Id, saved.CategoryId);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var from = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var to = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, from.Id);

        var result = await Mediator.Send(
            ItemCommandFactory.CreateMoveItemToCategoryCommand(item.Id, to.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnItemNotFound_WhenItemMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateMoveItemToCategoryCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.ItemNotFound);
    }
}
