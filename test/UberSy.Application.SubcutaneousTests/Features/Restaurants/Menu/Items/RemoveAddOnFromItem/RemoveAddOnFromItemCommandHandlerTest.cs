using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants.Items;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Items.RemoveAddOnFromItem;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class RemoveAddOnFromItemCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldRemoveAddOnFromItem_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);
        await RestaurantTestData.AssignAddOnAsync(Context, item, addOn);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateRemoveAddOnFromItemCommand(item.Id, addOn.Id), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbItems.AsNoTracking().Include(i => i.AddOns).FirstAsync(i => i.Id == item.Id);
        Assert.Empty(saved.AddOns);

        // The add-on itself must still exist (only the link is removed)
        Assert.True(await Context.TbAddOns.AsNoTracking().AnyAsync(a => a.Id == addOn.Id));
    }

    [Fact]
    public async Task Handle_ShouldReturnNotAssigned_WhenAddOnNotOnItem()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateRemoveAddOnFromItemCommand(item.Id, addOn.Id), CancellationToken.None);

        AssertError(result, ItemErrors.AddOnNotAssigned);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);
        await RestaurantTestData.AssignAddOnAsync(Context, item, addOn);
        AsOwner2();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateRemoveAddOnFromItemCommand(item.Id, addOn.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);

        var saved = await Context.TbItems.AsNoTracking().Include(i => i.AddOns).FirstAsync(i => i.Id == item.Id);
        Assert.Single(saved.AddOns);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);

        var result = await Mediator.Send(
            ItemCommandFactory.CreateRemoveAddOnFromItemCommand(item.Id, addOn.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnItemNotFound_WhenItemMissing()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateRemoveAddOnFromItemCommand(Guid.NewGuid(), addOn.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.ItemNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnAddOnNotFound_WhenAddOnMissing()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateRemoveAddOnFromItemCommand(item.Id, Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.AddOnNotFound);
    }
}
