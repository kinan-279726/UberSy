using UberSy.Application.Common.Errors;
using UberSy.Application.Features.Restaurants.Menu.Categories.Quiries.GetCategoryById;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Categories.Queries;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class GetCategoryByIdQueryHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldReturnCategory_WhenExists()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var printer = await RestaurantTestData.SeedPrinterAsync(Context, restaurant.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(
            Context, restaurant.Id, printerId: printer.Id, sortOrder: 4, nameAr: "المقبلات", nameEn: "Starters");

        var result = await Mediator.Send(new GetCategoryByIdQuery(category.Id), CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(category.Id, result.Value.Id);
        Assert.Equal(restaurant.Id, result.Value.RestaurantId);
        Assert.Equal(printer.Id, result.Value.PrinterId);
        Assert.Equal("المقبلات", result.Value.NameAr);
        Assert.Equal("Starters", result.Value.NameEn);
        Assert.Equal((byte)4, result.Value.SortOrder);
        Assert.True(result.Value.IsActive);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenMissing()
    {
        var result = await Mediator.Send(new GetCategoryByIdQuery(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.CategoryNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenIdEmpty()
    {
        var result = await Mediator.Send(new GetCategoryByIdQuery(Guid.Empty), CancellationToken.None);

        AssertErrorCode(result, "CategoryId");
    }
}
