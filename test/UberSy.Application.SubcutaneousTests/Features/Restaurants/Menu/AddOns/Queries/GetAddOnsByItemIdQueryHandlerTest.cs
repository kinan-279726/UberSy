using UberSy.Application.Common.Errors;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Queries.GetAddOnsByItemId;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.AddOns.Queries;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetAddOnsByItemIdQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldReturnOnlyActiveAddOnsAssignedToItem()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        var active = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: true);
        var inactive = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: false);
        await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: true); // not assigned to the item
        await RestaurantTestData.AssignAddOnAsync(Context, item, active);
        await RestaurantTestData.AssignAddOnAsync(Context, item, inactive);

        var result = await Mediator.Send(new GetAddOnsByItemIdQuery(item.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(active.Id, Assert.Single(result.Value).Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenItemHasNoAddOns()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);

        var result = await Mediator.Send(new GetAddOnsByItemIdQuery(item.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnItemNotFound_WhenItemMissing()
    {
        var result = await Mediator.Send(new GetAddOnsByItemIdQuery(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.ItemNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenItemIdEmpty()
    {
        var result = await Mediator.Send(new GetAddOnsByItemIdQuery(Guid.Empty), CancellationToken.None);

        AssertErrorCode(result, "ItemId");
    }
}
