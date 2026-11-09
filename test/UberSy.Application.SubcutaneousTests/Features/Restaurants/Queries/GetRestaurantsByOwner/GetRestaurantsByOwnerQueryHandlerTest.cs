using UberSy.Application.Common.Errors;
using UberSy.Application.Features.Restaurants.Queries.GetRestaurantsByOwner;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Queries.GetRestaurantsByOwner;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetRestaurantsByOwnerQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldReturnOnlyCurrentUsersRestaurants()
    {
        await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, nameEn: "Owner1 A");
        await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: true, nameEn: "Owner1 B");
        await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner2.Id, nameEn: "Owner2 A");
        AsOwner1();

        var result = await Mediator.Send(new GetRestaurantsByOwnerQuery(), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(2, result.Value.Count);
        Assert.Contains(result.Value, r => r.NameEn == "Owner1 A");
        Assert.Contains(result.Value, r => r.NameEn == "Owner1 B");
        Assert.DoesNotContain(result.Value, r => r.NameEn == "Owner2 A");
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenUserOwnsNothing()
    {
        await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner2();

        var result = await Mediator.Send(new GetRestaurantsByOwnerQuery(), CancellationToken.None);

        AssertSuccess(result);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var result = await Mediator.Send(new GetRestaurantsByOwnerQuery(), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }
}
