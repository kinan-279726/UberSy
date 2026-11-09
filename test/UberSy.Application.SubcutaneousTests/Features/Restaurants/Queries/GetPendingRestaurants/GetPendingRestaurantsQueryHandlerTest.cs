using UberSy.Application.Features.Restaurants.Queries.GetPendingRestaurants;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Queries.GetPendingRestaurants;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetPendingRestaurantsQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldReturnOnlyNotApprovedRestaurants()
    {
        AsManager();

        await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: false, nameEn: "Pending One");
        await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: true, nameEn: "Approved One");

        var result = await Mediator.Send(new GetPendingRestaurantsQuery(), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal("Pending One", Assert.Single(result.Value.Items).NameEn);
        Assert.False(result.Value.Items[0].IsApproved);
        Assert.Equal(1, result.Value.TotalCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnOldestFirst()
    {
        AsManager();

        await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, nameEn: "First");
        await Task.Delay(50); // make sure CreatedAtUtc differs
        await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, nameEn: "Second");

        var result = await Mediator.Send(new GetPendingRestaurantsQuery(), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(["First", "Second"], result.Value.Items.Select(r => r.NameEn).ToArray());
    }

    [Fact]
    public async Task Handle_ShouldPageResults()
    {
        AsManager();

        for (var i = 1; i <= 3; i++)
            await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, nameEn: $"Pending {i}");

        var result = await Mediator.Send(new GetPendingRestaurantsQuery(PageNumber: 2, PageSize: 2), CancellationToken.None);

        AssertSuccess(result);
        Assert.Single(result.Value.Items);
        Assert.Equal(3, result.Value.TotalCount);
        Assert.Equal(2, result.Value.TotalPages);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPage_WhenNothingPending()
    {
        AsManager();

        var result = await Mediator.Send(new GetPendingRestaurantsQuery(), CancellationToken.None);

        AssertSuccess(result);
        Assert.Empty(result.Value.Items);
    }

    [Fact]
    public async Task Handle_ShouldRejectNonManager()
    {
        // KNOWN GAP: the handler has no manager check, so any caller (even anonymous) can list pending restaurants.
        // If the controller is protected by [Authorize(Roles = "Admin")] you can delete this test.
        await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsCustomer();

        var result = await Mediator.Send(new GetPendingRestaurantsQuery(), CancellationToken.None);

        Assert.True(result.IsError, "Pending restaurants must only be visible to the manager.");
    }
}
