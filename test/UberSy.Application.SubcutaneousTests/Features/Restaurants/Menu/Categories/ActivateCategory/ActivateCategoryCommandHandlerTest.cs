using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants.Categories;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Categories.ActivateCategory;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class ActivateCategoryCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldActivateCategoryAndItsItems_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id, active: false);
        await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id, active: false);
        await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id, active: false);
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateActivateCategoryCommand(category.Id), CancellationToken.None);

        AssertSuccess(result);

        var savedCategory = await Context.TbCategories.AsNoTracking().FirstAsync(c => c.Id == category.Id);
        Assert.True(savedCategory.IsActive);

        var items = await Context.TbItems.AsNoTracking().Where(i => i.CategoryId == category.Id).ToListAsync();
        Assert.Equal(2, items.Count);
        Assert.All(items, i => Assert.True(i.IsActive));
    }

    [Fact]
    public async Task Handle_ShouldNotTouchItemsOfOtherCategories()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id, active: false);
        var otherCategory = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id, active: true);
        var otherItem = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, otherCategory.Id, active: false);
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateActivateCategoryCommand(category.Id), CancellationToken.None);

        AssertSuccess(result);

        var savedOther = await Context.TbItems.AsNoTracking().FirstAsync(i => i.Id == otherItem.Id);
        Assert.False(savedOther.IsActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id, active: false);
        AsOwner2();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateActivateCategoryCommand(category.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);

        var saved = await Context.TbCategories.AsNoTracking().FirstAsync(c => c.Id == category.Id);
        Assert.False(saved.IsActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id, active: false);

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateActivateCategoryCommand(category.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnCategoryNotFound_WhenMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateActivateCategoryCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.CategoryNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnAlreadyActive_WhenCategoryActive()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id, active: true);
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateActivateCategoryCommand(category.Id), CancellationToken.None);

        AssertError(result, CategoryErrors.CategoryAlreadyActive);
    }
}
