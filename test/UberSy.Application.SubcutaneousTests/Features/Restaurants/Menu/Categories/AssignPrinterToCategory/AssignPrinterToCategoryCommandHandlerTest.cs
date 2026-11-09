using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Categories.AssignPrinterToCategory;

//[Collection(WebAppFactoryCollection.CollectionName)]
//public sealed class AssignPrinterToCategoryCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
//{
//    [Fact]
//    public async Task Handle_ShouldAssignPrinter_WhenPrinterBelongsToSameRestaurant()
//    {
//        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
//        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
//        var printer = await RestaurantTestData.SeedPrinterAsync(Context, restaurant.Id);
//        AsOwner1();

//        var result = await Mediator.Send(
//            CategoryCommandFactory.CreateAssignPrinterToCategoryCommand(category.Id, printer.Id), CancellationToken.None);

//        AssertSuccess(result);

//        var saved = await Context.TbCategories.AsNoTracking().FirstAsync(c => c.Id == category.Id);
//        Assert.Equal(printer.Id, saved.PrinterId);
//    }

//    [Fact]
//    public async Task Handle_ShouldReturnCategoryNotFound_WhenCategoryMissing()
//    {
//        AsOwner1();

//        var result = await Mediator.Send(
//            CategoryCommandFactory.CreateAssignPrinterToCategoryCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

//        AssertError(result, ApplicationErrors.CategoryNotFound);
//    }

//    [Fact]
//    public async Task Handle_ShouldReturnPrinterNotFound_WhenPrinterMissing()
//    {
//        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
//        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
//        AsOwner1();

//        var result = await Mediator.Send(
//            CategoryCommandFactory.CreateAssignPrinterToCategoryCommand(category.Id, Guid.NewGuid()), CancellationToken.None);

//        AssertError(result, ApplicationErrors.PrinterNotFound);
//    }

//    [Fact]
//    public async Task Handle_ShouldReturnPrinterNotFound_WhenPrinterBelongsToAnotherRestaurant()
//    {
//        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
//        var otherRestaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner2.Id);
//        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
//        var foreignPrinter = await RestaurantTestData.SeedPrinterAsync(Context, otherRestaurant.Id);
//        AsOwner1();

//        var result = await Mediator.Send(
//            CategoryCommandFactory.CreateAssignPrinterToCategoryCommand(category.Id, foreignPrinter.Id), CancellationToken.None);

//        AssertError(result, ApplicationErrors.PrinterNotFound);

//        var saved = await Context.TbCategories.AsNoTracking().FirstAsync(c => c.Id == category.Id);
//        Assert.Null(saved.PrinterId);
//    }

//    [Fact]
//    public async Task Handle_ShouldRejectAnotherOwner()
//    {
//        // KNOWN GAP: this handler has no ownership check, so any user can change another restaurant's category.
//        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
//        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
//        var printer = await RestaurantTestData.SeedPrinterAsync(Context, restaurant.Id);
//        AsOwner2();

//        var result = await Mediator.Send(
//            CategoryCommandFactory.CreateAssignPrinterToCategoryCommand(category.Id, printer.Id), CancellationToken.None);

//        Assert.True(result.IsError, "A user who does not own the restaurant must not be able to assign printers.");

//        var saved = await Context.TbCategories.AsNoTracking().FirstAsync(c => c.Id == category.Id);
//        Assert.Null(saved.PrinterId);
//    }

//    [Fact]
//    public async Task Handle_ShouldRejectUnauthenticatedUser()
//    {
//        // KNOWN GAP: no authentication / ownership check in the handler (see above)
//        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
//        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
//        var printer = await RestaurantTestData.SeedPrinterAsync(Context, restaurant.Id);

//        var result = await Mediator.Send(
//            CategoryCommandFactory.CreateAssignPrinterToCategoryCommand(category.Id, printer.Id), CancellationToken.None);

//        Assert.True(result.IsError, "An unauthenticated user must not be able to assign printers.");
//    }
//}
