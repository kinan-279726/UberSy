using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants.Items;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Items.AssignAddOnToItem;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class AssignAddOnToItemCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldAssignAddOn_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateAssignAddOnToItemCommand(item.Id, addOn.Id), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbItems.AsNoTracking().Include(i => i.AddOns).FirstAsync(i => i.Id == item.Id);
        Assert.Single(saved.AddOns);
        Assert.Equal(addOn.Id, saved.AddOns.First().Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyAssigned_WhenAddOnAlreadyOnItem()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);
        await RestaurantTestData.AssignAddOnAsync(Context, item, addOn);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateAssignAddOnToItemCommand(item.Id, addOn.Id), CancellationToken.None);

        AssertError(result, ItemErrors.AddOnAlreadyAssigned);
    }

    [Fact]
    public async Task Handle_ShouldReturnDomainError_WhenAddOnBelongsToAnotherRestaurant()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var otherRestaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner2.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        var foreignAddOn = await RestaurantTestData.SeedAddOnAsync(Context, otherRestaurant.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateAssignAddOnToItemCommand(item.Id, foreignAddOn.Id), CancellationToken.None);

        AssertError(result, ItemErrors.AddOnFromDifferentRestaurant);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);
        AsOwner2();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateAssignAddOnToItemCommand(item.Id, addOn.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);

        var result = await Mediator.Send(
            ItemCommandFactory.CreateAssignAddOnToItemCommand(item.Id, addOn.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnItemNotFound_WhenItemMissing()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateAssignAddOnToItemCommand(Guid.NewGuid(), addOn.Id), CancellationToken.None);

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
            ItemCommandFactory.CreateAssignAddOnToItemCommand(item.Id, Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.AddOnNotFound);
    }
}
