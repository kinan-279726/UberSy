using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants.Items;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Items.CreateItem;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class CreateItemCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldCreateActiveItem_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateCreateItemCommand(
                restaurant.Id, category.Id, price: 42.5m, calories: 500, sortOrder: 4),
            CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(restaurant.Id, result.Value.RestaurantId);
        Assert.Equal(category.Id, result.Value.CategoryId);

        var saved = await Context.TbItems.AsNoTracking().FirstAsync(i => i.Id == result.Value.Id);
        Assert.Equal(42.5m, saved.Price);
        Assert.Equal(500, saved.Calories);
        Assert.Equal((byte)4, saved.SortOrder);
        Assert.True(saved.IsActive);
        Assert.Equal(restaurant.Id, saved.RestaurantId);
        Assert.Equal(category.Id, saved.CategoryId);
    }

    [Fact]
    public async Task Handle_ShouldCreateItem_WhenManager()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        AsManager();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateCreateItemCommand(restaurant.Id, category.Id), CancellationToken.None);

        AssertSuccess(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        AsOwner2();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateCreateItemCommand(restaurant.Id, category.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);
        Assert.False(await Context.TbItems.AsNoTracking().AnyAsync());
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);

        var result = await Mediator.Send(
            ItemCommandFactory.CreateCreateItemCommand(restaurant.Id, category.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenRestaurantMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateCreateItemCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnCategoryNotFound_WhenCategoryMissing()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateCreateItemCommand(restaurant.Id, Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.CategoryNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnCategoryNotFound_WhenCategoryBelongsToAnotherRestaurant()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var otherRestaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner2.Id);
        var foreignCategory = await RestaurantTestData.SeedCategoryAsync(Context, otherRestaurant.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateCreateItemCommand(restaurant.Id, foreignCategory.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.CategoryNotFound);
        Assert.False(await Context.TbItems.AsNoTracking().AnyAsync());
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenPriceNotPositive()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateCreateItemCommand(restaurant.Id, category.Id, price: 0m), CancellationToken.None);

        AssertErrorCode(result, "Price");
    }

    [Fact]
    public async Task Handle_ShouldReturnDomainError_WhenNameTooShort()
    {
        // Passes the validator (NotEmpty) but the domain requires at least 2 characters
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateCreateItemCommand(restaurant.Id, category.Id, nameEn: "A"), CancellationToken.None);

        AssertError(result, ItemErrors.NameEnTooShort);
    }
}
