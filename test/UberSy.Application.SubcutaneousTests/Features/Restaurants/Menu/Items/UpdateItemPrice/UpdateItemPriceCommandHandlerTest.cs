using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Items.UpdateItemPrice;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class UpdateItemPriceCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldUpdatePrice_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id, price: 25m);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateUpdateItemPriceCommand(item.Id, 31.75m), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbItems.AsNoTracking().FirstAsync(i => i.Id == item.Id);
        Assert.Equal(31.75m, saved.Price);
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id, price: 25m);
        AsOwner2();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateUpdateItemPriceCommand(item.Id, 99m), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);

        var saved = await Context.TbItems.AsNoTracking().FirstAsync(i => i.Id == item.Id);
        Assert.Equal(25m, saved.Price);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);

        var result = await Mediator.Send(ItemCommandFactory.CreateUpdateItemPriceCommand(item.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnItemNotFound_WhenMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(ItemCommandFactory.CreateUpdateItemPriceCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.ItemNotFound);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public async Task Handle_ShouldReturnValidationError_WhenPriceNotPositive(double price)
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateUpdateItemPriceCommand(item.Id, (decimal)price), CancellationToken.None);

        AssertErrorCode(result, "Price");
    }
}
