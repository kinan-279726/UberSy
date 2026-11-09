using UberSy.Application.Features.Restaurants.Menu.Items.Queries.GetItemsByRestaurant;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Items.Queries;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetItemsByRestaurantQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    // Each test uses its own restaurant id, so the cached query (key = "items{RestaurantId}") never collides.

    [Fact]
    public async Task Handle_ShouldReturnActiveItemsOfRestaurantOrderedBySortOrder()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var second = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id, sortOrder: 2);
        var first = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id, sortOrder: 1);
        await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id, active: false, sortOrder: 3);

        var result = await Mediator.Send(new GetItemsByRestaurantQuery(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal([first.Id, second.Id], result.Value.Select(i => i.Id).ToArray());
    }

    [Fact]
    public async Task Handle_ShouldNotReturnItemsOfOtherRestaurants()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var other = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner2.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var otherCategory = await RestaurantTestData.SeedCategoryAsync(Context, other.Id);
        var mine = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        await RestaurantTestData.SeedItemAsync(Context, other.Id, otherCategory.Id);

        var result = await Mediator.Send(new GetItemsByRestaurantQuery(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(mine.Id, Assert.Single(result.Value).Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenRestaurantHasNoItems()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);

        var result = await Mediator.Send(new GetItemsByRestaurantQuery(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenRestaurantIdEmpty()
    {
        var result = await Mediator.Send(new GetItemsByRestaurantQuery(Guid.Empty), CancellationToken.None);

        AssertErrorCode(result, "RestaurantId");
    }
}
