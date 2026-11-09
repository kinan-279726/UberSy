using UberSy.Application.Common.Errors;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Queries.GetAddOnById;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.AddOns.Queries;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetAddOnByIdQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldReturnAddOn_WhenExists()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, price: 4m);

        var result = await Mediator.Send(new GetAddOnByIdQuery(addOn.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(addOn.Id, result.Value.Id);
        Assert.Equal(restaurant.Id, result.Value.RestaurantId);
        Assert.Equal("Extra Cheese", result.Value.NameEn);
        Assert.Equal(4m, result.Value.Price);
        Assert.True(result.Value.IsActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnInactiveAddOn_Too()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var addOn = await RestaurantTestData.SeedAddOnAsync(Context, restaurant.Id, active: false);

        var result = await Mediator.Send(new GetAddOnByIdQuery(addOn.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.False(result.Value.IsActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnAddOnNotFound_WhenMissing()
    {
        // KNOWN BUG: the handler returns ApplicationErrors.ItemNotFound instead of AddOnNotFound
        var result = await Mediator.Send(new GetAddOnByIdQuery(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.AddOnNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenIdEmpty()
    {
        var result = await Mediator.Send(new GetAddOnByIdQuery(Guid.Empty), CancellationToken.None);

        AssertErrorCode(result, "AddOnId");
    }
}
