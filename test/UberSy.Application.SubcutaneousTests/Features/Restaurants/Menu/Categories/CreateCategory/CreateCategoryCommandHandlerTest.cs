using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants.Categories;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Categories.CreateCategory;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class CreateCategoryCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldCreateActiveCategory_WhenOwner()
    {
        // KNOWN BUG: the handler calls EnsureIsOwnerByRestaurantIdAsync(restaurant.OwnerId) - an owner id where a
        // restaurant id is expected - so even the real owner gets Restaurant_NotFound.
        // Fix: EnsureIsOwnerByOwnerIdAsync(restaurant.OwnerId) (like CreateItem / CreateAddOn do).
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateCreateCategoryCommand(restaurant.Id, nameAr: "المشاوي", nameEn: "Grills", sortOrder: 3),
            CancellationToken.None);

        AssertSuccess(result);
        Assert.Equal(restaurant.Id, result.Value.RestaurantId);
        Assert.True(result.Value.IsActive);

        var saved = await Context.TbCategories.AsNoTracking().FirstAsync(c => c.Id == result.Value.Id);
        Assert.Equal("المشاوي", saved.NameAr);
        Assert.Equal("Grills", saved.NameEn);
        Assert.Equal((byte)3, saved.SortOrder);
        Assert.True(saved.IsActive);
        Assert.Null(saved.PrinterId);
    }

    [Fact]
    public async Task Handle_ShouldCreateCategoryWithPrinter_WhenPrinterProvided()
    {
        // Same KNOWN BUG as above applies here
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var printer = await RestaurantTestData.SeedPrinterAsync(Context, restaurant.Id);
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateCreateCategoryCommand(restaurant.Id, printer.Id), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbCategories.AsNoTracking().FirstAsync(c => c.Id == result.Value.Id);
        Assert.Equal(printer.Id, saved.PrinterId);
    }

    [Fact]
    public async Task Handle_ShouldRejectAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner2();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateCreateCategoryCommand(restaurant.Id), CancellationToken.None);

        Assert.True(result.IsError);
        Assert.False(await Context.TbCategories.AsNoTracking().AnyAsync());
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateCreateCategoryCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenRestaurantMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateCreateCategoryCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenNameArEmpty()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateCreateCategoryCommand(restaurant.Id, nameAr: ""), CancellationToken.None);

        AssertErrorCode(result, "NameAr");
    }

    [Fact]
    public async Task Handle_ShouldReturnDomainError_WhenNameTooShort()
    {
        // Passes the validator (NotEmpty) but the domain requires at least 2 characters.
        // Only reachable once the ownership bug above is fixed.
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateCreateCategoryCommand(restaurant.Id, nameEn: "A"), CancellationToken.None);

        AssertError(result, CategoryErrors.NameEnTooShort);
    }
}
