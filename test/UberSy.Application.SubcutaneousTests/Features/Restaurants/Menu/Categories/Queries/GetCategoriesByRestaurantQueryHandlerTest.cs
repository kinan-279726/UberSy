using UberSy.Application.Features.Restaurants.Menu.Categories.Quiries.GetCategoriesByRestaurant;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Categories.Queries;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetCategoriesByRestaurantQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    // Each test uses its own restaurant id, so the cached query (key = "category{RestaurantId}") never collides.

    [Fact]
    public async Task Handle_ShouldReturnCategoriesOrderedBySortOrder()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id, sortOrder: 3, nameEn: "Third");
        await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id, sortOrder: 1, nameEn: "First");
        await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id, sortOrder: 2, nameEn: "Second");

        var result = await Mediator.Send(new GetCategoriesByRestaurantQuery(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(["First", "Second", "Third"], result.Value.Select(c => c.NameEn).ToArray());
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyCategoriesOfThatRestaurant()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var other = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner2.Id);
        await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id, nameEn: "Mine");
        await RestaurantTestData.SeedCategoryAsync(Context, other.Id, nameEn: "Theirs");

        var result = await Mediator.Send(new GetCategoriesByRestaurantQuery(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal("Mine", Assert.Single(result.Value).NameEn);
    }

    [Fact]
    public async Task Handle_ShouldIncludeInactiveCategories()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id, active: false, nameEn: "Hidden");

        var result = await Mediator.Send(new GetCategoriesByRestaurantQuery(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);
        var category = Assert.Single(result.Value);
        Assert.False(category.IsActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenRestaurantHasNoCategories()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);

        var result = await Mediator.Send(new GetCategoriesByRestaurantQuery(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenRestaurantIdEmpty()
    {
        var result = await Mediator.Send(new GetCategoriesByRestaurantQuery(Guid.Empty), CancellationToken.None);

        AssertErrorCode(result, "RestaurantId");
    }
}
