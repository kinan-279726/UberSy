using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Domain.Restaurants;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.CreateRestaurant;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class CreateRestaurantCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldCreatePendingRestaurantOwnedByCurrentUser_WhenValid()
    {
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateRestaurantCommand(
                nameAr: "مطعم الشام", nameEn: "Al Sham", latitude: 33.5138, longitude: 36.2765),
            CancellationToken.None);

        AssertSuccess(result);
        Assert.NotEqual(Guid.Empty, result.Value.Id);
        Assert.False(result.Value.IsApproved);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == result.Value.Id);
        Assert.Equal(TestUsers.Owner1.Id, saved.OwnerId);
        Assert.Equal("مطعم الشام", saved.NameAr);
        Assert.Equal("Al Sham", saved.NameEn);
        Assert.False(saved.IsApproved);
        Assert.False(saved.IsOpenNow);
        Assert.Equal(36.2765, saved.Boundary.X, 4); // X = longitude
        Assert.Equal(33.5138, saved.Boundary.Y, 4); // Y = latitude
        Assert.Equal(4326, saved.Boundary.SRID);
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var result = await Mediator.Send(RestaurantCommandFactory.CreateRestaurantCommand(), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
        Assert.False(await Context.TbRestaurants.AsNoTracking().AnyAsync());
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenNameArEmpty()
    {
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateRestaurantCommand(nameAr: ""), CancellationToken.None);

        AssertErrorCode(result, "NameAr");
        Assert.False(await Context.TbRestaurants.AsNoTracking().AnyAsync());
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationError_WhenClosingTimeBeforeOpeningTime()
    {
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateRestaurantCommand(
                openingTime: new TimeOnly(22, 0), closingTime: new TimeOnly(9, 0)),
            CancellationToken.None);

        AssertErrorCode(result, "ClosingTime");
    }

    [Theory]
    [InlineData("0912345678")]      // missing country code
    [InlineData("+963812345678")]   // must start with +9639
    [InlineData("+96391234567")]    // one digit short
    public async Task Handle_ShouldReturnDomainError_WhenPhoneFormatInvalid(string phone)
    {
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateRestaurantCommand(phone: phone), CancellationToken.None);

        AssertError(result, RestaurantErrors.PhoneInvalid);
        Assert.False(await Context.TbRestaurants.AsNoTracking().AnyAsync());
    }

    [Fact]
    public async Task Handle_ShouldReturnDomainError_WhenNameTooShort()
    {
        AsOwner1();

        // Passes the validator (NotEmpty) but the domain requires at least 2 characters
        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateRestaurantCommand(nameEn: "A"), CancellationToken.None);

        AssertError(result, RestaurantErrors.NameEnTooShort);
    }

    [Fact]
    public async Task Handle_ShouldReturnDomainError_WhenTaxRateOutOfRange()
    {
        AsOwner1();

        // The validator has no TaxRate rule, so the domain rule is the only protection
        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateRestaurantCommand(taxRate: 150m), CancellationToken.None);

        AssertError(result, RestaurantErrors.TaxRateExceedsMaximum);
    }
}
