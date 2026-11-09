using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.SubcutaneousTests.Common;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.UpdateTaxRate;

[Collection(WebAppFactoryCollection.CollectionName)]
public sealed class UpdateTaxRateCommandHandlerTest(WebAppFactory factory) : AppTestBase(factory)
{
    [Fact]
    public async Task Handle_ShouldUpdateTaxRate_WhenOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateTaxRateCommand(restaurant.Id, 15m), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.Equal(15m, saved.TaxRate);
    }

    [Fact]
    public async Task Handle_ShouldAllowZeroTaxRate()
    {
        // KNOWN BUG: see UpdateTaxRateCommandValidatorTest (NotEmpty on decimal rejects 0)
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateTaxRateCommand(restaurant.Id, 0m), CancellationToken.None);

        AssertSuccess(result);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.Equal(0m, saved.TaxRate);
    }

    [Fact]
    public async Task Handle_ShouldUpdateTaxRate_WhenManager()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsManager();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateTaxRateCommand(restaurant.Id, 8m), CancellationToken.None);

        AssertSuccess(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnAccessDenied_WhenAnotherOwner()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner2();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateTaxRateCommand(restaurant.Id, 20m), CancellationToken.None);

        AssertError(result, ApplicationErrors.AccessDenied);

        var saved = await Context.TbRestaurants.AsNoTracking().FirstAsync(r => r.Id == restaurant.Id);
        Assert.Equal(5m, saved.TaxRate); // factory default
    }

    [Fact]
    public async Task Handle_ShouldReturnUnauthorized_WhenUnauthenticated()
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateTaxRateCommand(restaurant.Id), CancellationToken.None);

        AssertError(result, ApplicationErrors.Unauthorized);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenRestaurantMissing()
    {
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateTaxRateCommand(Guid.NewGuid()), CancellationToken.None);

        AssertError(result, ApplicationErrors.RestaurantNotFound);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public async Task Handle_ShouldReturnValidationError_WhenRateOutOfRange(double rate)
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(Context, TestUsers.Owner1.Id);
        AsOwner1();

        var result = await Mediator.Send(
            RestaurantCommandFactory.CreateUpdateTaxRateCommand(restaurant.Id, (decimal)rate), CancellationToken.None);

        AssertErrorCode(result, "TaxRate");
    }
}
