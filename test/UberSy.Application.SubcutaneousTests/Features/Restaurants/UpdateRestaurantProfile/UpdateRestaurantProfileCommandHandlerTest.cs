using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.UpdateRestaurantProfile;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class UpdateRestaurantProfileCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldUpdateProfile_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantProfileCommand(
                restaurant.Id,
                nameAr: "  مطعم الشام  ",
                nameEn: "Al Sham",
                descriptionAr: "وصف جديد",
                descriptionEn: "New description",
                phone: "+963987654321",
                email: "new@alsham.com"),
            CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.Equal("مطعم الشام", saved.NameAr); // trimmed by the domain
        Assert.Equal("Al Sham", saved.NameEn);
        Assert.Equal("وصف جديد", saved.DescriptionAr);
        Assert.Equal("New description", saved.DescriptionEn);
        Assert.Equal("+963987654321", saved.Phone);
        Assert.Equal("new@alsham.com", saved.Email);
    }

    [Fact]
    public async Task Handle_ShouldUpdateProfile_WhenManager()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsManager();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantProfileCommand(restaurant.Id), CancellationToken.None);

        AssertSuccess(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner2();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantProfileCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.Equal("Test Restaurant", saved.NameEn); // unchanged
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantProfileCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenRestaurantMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantProfileCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenNameEnEmpty()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantProfileCommand(restaurant.Id, nameEn: ""), CancellationToken.None);

        AssertErrorCode(result, "NameEn");
    }

    [Theory]
    [InlineData("0912345678")]
    [InlineData("+963812345678")]
    public async Task Handle_ShouldReturnDomainError_WhenPhoneFormatInvalid(string phone)
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateRestaurantProfileCommand(restaurant.Id, phone: phone), CancellationToken.None);

        AssertError(result, RestaurantErrors.PhoneInvalid);
    }
}
