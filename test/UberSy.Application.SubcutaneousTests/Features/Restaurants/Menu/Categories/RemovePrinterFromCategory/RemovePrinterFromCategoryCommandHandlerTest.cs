using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Categories.RemovePrinterFromCategory;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class RemovePrinterFromCategoryCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldRemovePrinter_WhenCategoryHasPrinter()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var printer = await RestaurantTestData.SeedPrinterAsync(Context, restaurant.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id, printerId: printer.Id);
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateRemovePrinterFromCategoryCommand(category.Id), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbCategories.AsNoTracking().FirstAsync(c => c.Id == category.Id);
        Assert.Null(saved.PrinterId);
    }

    [Fact]
    public async Task Handle_ShouldSucceed_WhenCategoryHasNoPrinter()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateRemovePrinterFromCategoryCommand(category.Id), CancellationToken.None);

        AssertSuccess(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnCategoryNotFound_WhenMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateRemovePrinterFromCategoryCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.CategoryNotFound);
    }

    [Fact]
    public async Task Handle_ShouldRejectAnotherOwner()
    {
        // KNOWN GAP: this handler has no ownership check
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var printer = await RestaurantTestData.SeedPrinterAsync(Context, restaurant.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id, printerId: printer.Id);
        AsOwner2();

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateRemovePrinterFromCategoryCommand(category.Id), CancellationToken.None);

        Assert.True(result.IsError, "A user who does not own the restaurant must not be able to remove printers.");

        var saved = await Context.TbCategories.AsNoTracking().FirstAsync(c => c.Id == category.Id);
        Assert.Equal(printer.Id, saved.PrinterId);
    }

    [Fact]
    public async Task Handle_ShouldRejectUnauthenticatedUser()
    {
        // KNOWN GAP: no authentication / ownership check in the handler
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);

        var result = await Mediator.Send(
            CategoryCommandFactory.CreateRemovePrinterFromCategoryCommand(category.Id), CancellationToken.None);

        Assert.True(result.IsError, "An unauthenticated user must not be able to remove printers.");
    }
}
