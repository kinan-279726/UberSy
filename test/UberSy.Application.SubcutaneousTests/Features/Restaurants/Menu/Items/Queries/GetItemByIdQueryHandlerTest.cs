using UberSy.Application.Common.Errors;
using UberSy.Application.Features.Restaurants.Menu.Items.Queries.GetItemById;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Items.Queries;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetItemByIdQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldReturnItem_WhenExists()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id, price: 30m);

        var result = await Mediator.Send(new GetItemByIdQuery(item.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(item.Id, result.Value.Id);
        Assert.Equal(restaurant.Id, result.Value.RestaurantId);
        Assert.Equal(category.Id, result.Value.CategoryId);
        Assert.Equal("Chicken Shawarma", result.Value.NameEn);
        Assert.Equal(30m, result.Value.Price);
        Assert.Equal(350, result.Value.Calories);
        Assert.Empty(result.Value.AddOns);
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyActiveAddOns_ByDefault()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        var activeAddOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: true);
        var inactiveAddOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: false);
        await RestaurantTestData.AssignAddOnAsync(Context, item, activeAddOn);
        await RestaurantTestData.AssignAddOnAsync(Context, item, inactiveAddOn);

        var result = await Mediator.Send(new GetItemByIdQuery(item.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(activeAddOn.Id, Assert.Single(result.Value.AddOns).Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnOnlyInactiveAddOns_WhenActiveFlagIsFalse()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        var activeAddOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: true);
        var inactiveAddOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: false);
        await RestaurantTestData.AssignAddOnAsync(Context, item, activeAddOn);
        await RestaurantTestData.AssignAddOnAsync(Context, item, inactiveAddOn);

        var result = await Mediator.Send(new GetItemByIdQuery(item.Id, Active: false), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(inactiveAddOn.Id, Assert.Single(result.Value.AddOns).Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnInactiveItem_Too()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id, active: false);

        var result = await Mediator.Send(new GetItemByIdQuery(item.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.False(result.Value.IsActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenMissing()
    {
        var result = await Mediator.Send(new GetItemByIdQuery(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.ItemNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenIdEmpty()
    {
        var result = await Mediator.Send(new GetItemByIdQuery(Guid.Empty), CancellationToken.None);

        AssertErrorCode(result, "ItemId");
    }
}
