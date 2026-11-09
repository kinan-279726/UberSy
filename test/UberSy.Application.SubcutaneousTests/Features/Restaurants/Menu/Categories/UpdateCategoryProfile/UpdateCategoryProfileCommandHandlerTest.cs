using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants.Categories;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Categories.UpdateCategoryProfile;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class UpdateCategoryProfileCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldUpdateNames_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateUpdateCategoryProfileCommand(category.Id, "  الحلويات  ", "Desserts"),
            CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbCategories.AsNoTracking().FirstAsync(c => c.Id == category.Id);
        Assert.Equal("الحلويات", saved.NameAr); // trimmed by the domain
        Assert.Equal("Desserts", saved.NameEn);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        AsOwner2();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateUpdateCategoryProfileCommand(category.Id, "الحلويات", "Desserts"),
            CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);

        var saved = await Context.TbCategories.AsNoTracking().FirstAsync(c => c.Id == category.Id);
        Assert.Equal("Grills", saved.NameEn); // factory default
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateUpdateCategoryProfileCommand(category.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnCategoryNotFound_WhenMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateUpdateCategoryProfileCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.CategoryNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenNameEnEmpty()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateUpdateCategoryProfileCommand(category.Id, nameEn: ""), CancellationToken.None);

        AssertErrorCode(result, "NameEn");
    }

    [Fact]
    public async Task Handle_ShouldReturnDomainError_WhenNameTooShort()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateUpdateCategoryProfileCommand(category.Id, nameAr: "أ"), CancellationToken.None);

        AssertError(result, CategoryErrors.NameArTooShort);
    }
}
