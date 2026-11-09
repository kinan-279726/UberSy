using UberSy.Application.Features.Restaurants.Menu.AddOns.Queries.GetAddOnsByRestaurant;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.AddOns.Queries;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetAddOnsByRestaurantQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldReturnOnlyActiveAddOnsOfRestaurant()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var other = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner2.Id);
        var active = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: true);
        await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: false);
        await RestaurantTestData.SeedAddOnAsync(Context, other.Id, active: true);

        var result = await Mediator.Send(new GetAddOnsByRestaurantQuery(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(active.Id, Assert.Single(result.Value).Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenRestaurantHasNoAddOns()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);

        var result = await Mediator.Send(new GetAddOnsByRestaurantQuery(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenRestaurantIdEmpty()
    {
        var result = await Mediator.Send(new GetAddOnsByRestaurantQuery(Guid.Empty), CancellationToken.None);

        AssertErrorCode(result, "RestaurantId");
    }
}
