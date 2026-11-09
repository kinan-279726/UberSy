using UberSy.Application.Common.Errors;
using UberSy.Application.Features.Restaurants.Menu.Items.Queries.GetItemsByCategory;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Items.Queries;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetItemsByCategoryQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    // Each test uses its own category id, so the cached query (key = "items{CategoryId}") never collides.

    [Fact]
    public async Task Handle_ShouldReturnActiveItemsOrderedBySortOrder()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var third = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id, sortOrder: 3);
        var first = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id, sortOrder: 1);
        var second = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id, sortOrder: 2);
        await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id, active: false, sortOrder: 4);

        var result = await Mediator.Send(new GetItemsByCategoryQuery(category.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal([first.Id, second.Id, third.Id], result.Value.Select(i => i.Id).ToArray());
    }

    [Fact]
    public async Task Handle_ShouldNotReturnItemsOfOtherCategories()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var other = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var mine = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, other.Id);

        var result = await Mediator.Send(new GetItemsByCategoryQuery(category.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(mine.Id, Assert.Single(result.Value).Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenCategoryHasNoActiveItems()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id, active: false);

        var result = await Mediator.Send(new GetItemsByCategoryQuery(category.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnCategoryNotFound_WhenCategoryMissing()
    {
        var result = await Mediator.Send(new GetItemsByCategoryQuery(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.CategoryNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenCategoryIdEmpty()
    {
        var result = await Mediator.Send(new GetItemsByCategoryQuery(Guid.Empty), CancellationToken.None);

        AssertErrorCode(result, "CategoryId");
    }
}
