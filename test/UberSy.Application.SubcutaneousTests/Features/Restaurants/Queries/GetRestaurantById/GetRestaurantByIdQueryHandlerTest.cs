using UberSy.Application.Common.Errors;
using UberSy.Application.Features.Restaurants.Queries.GetRestaurantById;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Queries.GetRestaurantById;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetRestaurantByIdQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldReturnRestaurant_WhenExists()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(
            Context, TestUsers.Owner1.Id, open: true, nameAr: "مطعم الشام", nameEn: "Al Sham");

        var result = await Mediator.Send(new GetRestaurantByIdQuery(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(restaurant.Id, result.Value.Id);
        Assert.Equal("مطعم الشام", result.Value.NameAr);
        Assert.Equal("Al Sham", result.Value.NameEn);
        Assert.Equal("+963912345678", result.Value.Phone);
        Assert.True(result.Value.IsApproved);
        Assert.True(result.Value.IsOpenNow);
    }

    [Fact]
    public async Task Handle_ShouldReturnPendingRestaurant_WithIsApprovedFalse()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);

        var result = await Mediator.Send(new GetRestaurantByIdQuery(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.False(result.Value.IsApproved);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenMissing()
    {
        var result = await Mediator.Send(new GetRestaurantByIdQuery(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }
}
