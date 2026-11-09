using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants.Categories;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Categories.DeactivateCategory;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class DeactivateCategoryCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldDeactivateCategoryAndItsItems_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateDeactivateCategoryCommand(category.Id), CancellationToken.None);

        AssertSuccess(result);

        var savedCategory = await Context.TbCategories.AsNoTracking().FirstAsync(c => c.Id == category.Id);
        Assert.False(savedCategory.IsActive);

        var items = await Context.TbItems.AsNoTracking().Where(i => i.CategoryId == category.Id).ToListAsync();
        Assert.Equal(2, items.Count);
        Assert.All(items, i => Assert.False(i.IsActive));
    }

    [Fact]
    public async Task Handle_ShouldNotTouchItemsOfOtherCategories()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var otherCategory = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var otherItem = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, otherCategory.Id);
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateDeactivateCategoryCommand(category.Id), CancellationToken.None);

        AssertSuccess(result);

        var savedOther = await Context.TbItems.AsNoTracking().FirstAsync(i => i.Id == otherItem.Id);
        Assert.True(savedOther.IsActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        AsOwner2();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateDeactivateCategoryCommand(category.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);

        var saved = await Context.TbCategories.AsNoTracking().FirstAsync(c => c.Id == category.Id);
        Assert.True(saved.IsActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateDeactivateCategoryCommand(category.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnCategoryNotFound_WhenMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateDeactivateCategoryCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.CategoryNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyInactive_WhenCategoryInactive()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id, active: false);
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateDeactivateCategoryCommand(category.Id), CancellationToken.None);

        AssertError(result, CategoryErrors.CategoryAlreadyInactive);
    }
}
