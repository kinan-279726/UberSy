using UberSy.Application.Features.Restaurants.Queries.GetAllRestaurant;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Queries.GetAllRestaurant;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetAllRestaurantQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldReturnOnlyApprovedRestaurants()
    {
        await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: true, nameEn: "Approved One");
        await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: false, nameEn: "Pending One");

        var result = await Mediator.Send(new GetAllRestaurantQuery(null, null), CancellationToken.None);

        AssertSuccess(result);
        Assert.Single(result.Value.Items);
        Assert.Equal("Approved One", result.Value.Items[0].NameEn);
        Assert.Equal(1, result.Value.TotalCount);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPage_WhenNoRestaurants()
    {
        var result = await Mediator.Send(new GetAllRestaurantQuery(null, null), CancellationToken.None);

        AssertSuccess(result);
        Assert.Empty(result.Value.Items);
        Assert.Equal(0, result.Value.TotalCount);
    }

    [Fact]
    public async Task Handle_ShouldFilterBySearch_InArabicOrEnglishName()
    {
        await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: true, nameAr: "مطعم الشام", nameEn: "Al Sham");
        await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: true, nameAr: "بيتزا هت", nameEn: "Pizza Place");

        var byEnglish = await Mediator.Send(new GetAllRestaurantQuery("Pizza", null), CancellationToken.None);
        var byArabic = await Mediator.Send(new GetAllRestaurantQuery("الشام", null), CancellationToken.None);

        AssertSuccess(byEnglish);
        AssertSuccess(byArabic);
        Assert.Equal("Pizza Place", Assert.Single(byEnglish.Value.Items).NameEn);
        Assert.Equal("Al Sham", Assert.Single(byArabic.Value.Items).NameEn);
    }

    [Fact]
    public async Task Handle_ShouldFilterByOpenState()
    {
        await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, open: true, nameEn: "Open Place");
        await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: true, nameEn: "Closed Place");

        var open = await Mediator.Send(new GetAllRestaurantQuery(null, true), CancellationToken.None);
        var closed = await Mediator.Send(new GetAllRestaurantQuery(null, false), CancellationToken.None);

        Assert.Equal("Open Place", Assert.Single(open.Value.Items).NameEn);
        Assert.Equal("Closed Place", Assert.Single(closed.Value.Items).NameEn);
    }

    [Fact]
    public async Task Handle_ShouldPutOpenRestaurantsFirst()
    {
        await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: true, nameAr: "أ مغلق", nameEn: "Closed");
        await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, open: true, nameAr: "ب مفتوح", nameEn: "Open");

        var result = await Mediator.Send(new GetAllRestaurantQuery(null, null), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal("Open", result.Value.Items[0].NameEn);
        Assert.Equal("Closed", result.Value.Items[1].NameEn);
    }

    [Fact]
    public async Task Handle_ShouldPageResults()
    {
        for (var i = 1; i <= 5; i++)
            await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id, approved: true, nameAr: $"مطعم {i}", nameEn: $"Place {i}");

        var page2 = await Mediator.Send(new GetAllRestaurantQuery(null, null, PageNumber: 2, PageSize: 2), CancellationToken.None);

        AssertSuccess(page2);
        Assert.Equal(2, page2.Value.Items.Count);
        Assert.Equal(5, page2.Value.TotalCount);
        Assert.Equal(3, page2.Value.TotalPages);
        Assert.Equal(2, page2.Value.PageNumber);
        Assert.True(page2.Value.HasNextPage);
        Assert.True(page2.Value.HasPreviousPage);
    }

    [Theory]
    [InlineData(0, 20, "PageNumber")]
    [InlineData(1, 0, "PageSize")]
    [InlineData(1, 51, "PageSize")]
    public async Task Handle_ShouldReturnValidationError_WhenPagingInvalid(int pageNumber, int pageSize, string expectedCode)
    {
        var result = await Mediator.Send(
            new GetAllRestaurantQuery(null, null, pageNumber, pageSize), CancellationToken.None);

        AssertErrorCode(result, expectedCode);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenSearchTooLong()
    {
        var result = await Mediator.Send(new GetAllRestaurantQuery(new string('a', 101), null), CancellationToken.None);

        AssertErrorCode(result, "Search");
    }
}
