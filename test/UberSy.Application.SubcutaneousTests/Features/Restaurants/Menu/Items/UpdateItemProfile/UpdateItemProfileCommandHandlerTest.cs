using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants.Items;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Items.UpdateItemProfile;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class UpdateItemProfileCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldUpdateProfile_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateUpdateItemProfileCommand(
                item.Id,
                nameAr: "  فروج مشوي  ",
                nameEn: "Grilled Chicken",
                descriptionAr: "فروج مشوي على الفحم",
                descriptionEn: "Charcoal grilled chicken",
                imageUrl: "https://cdn.ubersy.com/items/chicken.jpg"),
            CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbItems.AsNoTracking().FirstAsync(i => i.Id == item.Id);
        Assert.Equal("فروج مشوي", saved.NameAr); // trimmed by the domain
        Assert.Equal("Grilled Chicken", saved.NameEn);
        Assert.Equal("فروج مشوي على الفحم", saved.DescriptionAr);
        Assert.Equal("Charcoal grilled chicken", saved.DescriptionEn);
        Assert.Equal("https://cdn.ubersy.com/items/chicken.jpg", saved.ImageUrl);
        Assert.Equal(25m, saved.Price); // untouched
    }

    [Fact]
    public async Task Handle_ShouldReturnRestaurantNotFound_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        AsOwner2();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateUpdateItemProfileCommand(item.Id, nameEn: "Hacked"), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);

        var saved = await Context.TbItems.AsNoTracking().FirstAsync(i => i.Id == item.Id);
        Assert.Equal("Chicken Shawarma", saved.NameEn);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);

        var result = await Mediator.Send(ItemCommandFactory.CreateUpdateItemProfileCommand(item.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnItemNotFound_WhenMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateUpdateItemProfileCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.ItemNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenNameArEmpty()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateUpdateItemProfileCommand(item.Id, nameAr: ""), CancellationToken.None);

        AssertErrorCode(result, "NameAr");
    }

    [Fact]
    public async Task Handle_ShouldReturnDomainError_WhenNameTooShort()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        var category = await RestaurantTestData.SeedCategoryAsync(Context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(Context, restaurant.Id, category.Id);
        AsOwner1();

        var result = await Mediator.Send(
            ItemCommandFactory.CreateUpdateItemProfileCommand(item.Id, nameEn: "A"), CancellationToken.None);

        AssertError(result, ItemErrors.NameEnTooShort);
    }
}
