using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.UpdateCategorySortOrder;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Categories.UpdateCategorySortOrder;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class UpdateCategorySortOrderCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldUpdateSortOrder_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id, sortOrder: 1);
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateUpdateCategorySortOrderCommand(category.Id, 7), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbCategories.AsNoTracking().FirstAsync(c => c.Id == category.Id);
        Assert.Equal((byte)7, saved.SortOrder);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id, sortOrder: 1);
        AsOwner2();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateUpdateCategorySortOrderCommand(category.Id, 9), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);

        var saved = await Context.TbCategories.AsNoTracking().FirstAsync(c => c.Id == category.Id);
        Assert.Equal((byte)1, saved.SortOrder);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateUpdateCategorySortOrderCommand(category.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnCategoryNotFound_WhenMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateUpdateCategorySortOrderCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.CategoryNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenCategoryIdEmpty()
    {
        AsOwner1();

        var result = await Mediator.Send(new UpdateCategorySortOrderCommand(Guid.Empty, 1), CancellationToken.None);

        AssertErrorCode(result, "CategoryId");
    }
}
