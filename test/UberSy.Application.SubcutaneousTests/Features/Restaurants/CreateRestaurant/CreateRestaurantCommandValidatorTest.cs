using UberSy.Application.Features.Restaurants.Commands.CreateRestaurant;
using UberSy.Test.Common.Restaurants;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.CreateRestaurant;

public sealed class CreateRestaurantCommandValidatorTest
{
    private readonly CreateRestaurantCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenCommandIsValid()
    {
        var result = _validator.Validate(RestaurantCommandFactory.CreateRestaurantCommand());

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldFail_WhenNameArEmpty(string name)
    {
        var result = _validator.Validate(RestaurantCommandFactory.CreateRestaurantCommand(nameAr: name));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameAr");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameArTooLong()
    {
        var result = _validator.Validate(RestaurantCommandFactory.CreateRestaurantCommand(nameAr: new string('أ', 101)));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameAr");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldFail_WhenNameEnEmpty(string name)
    {
        var result = _validator.Validate(RestaurantCommandFactory.CreateRestaurantCommand(nameEn: name));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameEn");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameEnTooLong()
    {
        var result = _validator.Validate(RestaurantCommandFactory.CreateRestaurantCommand(nameEn: new string('a', 101)));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameEn");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldFail_WhenPhoneEmpty(string phone)
    {
        var result = _validator.Validate(RestaurantCommandFactory.CreateRestaurantCommand(phone: phone));

        Assert.Contains(result.Errors, e => e.PropertyName == "Phone");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPhoneTooLong()
    {
        var result = _validator.Validate(RestaurantCommandFactory.CreateRestaurantCommand(phone: new string('1', 21)));

        Assert.Contains(result.Errors, e => e.PropertyName == "Phone");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Validate_ShouldFail_WhenCommissionRateOutOfRange(double rate)
    {
        var result = _validator.Validate(RestaurantCommandFactory.CreateRestaurantCommand(commissionRate: (decimal)rate));

        Assert.Contains(result.Errors, e => e.PropertyName == "CommissionRate");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    public void Validate_ShouldPass_WhenCommissionRateAtBoundaries(double rate)
    {
        var result = _validator.Validate(RestaurantCommandFactory.CreateRestaurantCommand(commissionRate: (decimal)rate));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenClosingTimeNotAfterOpeningTime()
    {
        var result = _validator.Validate(RestaurantCommandFactory.CreateRestaurantCommand(
            openingTime: new TimeOnly(10, 0), closingTime: new TimeOnly(10, 0)));

        Assert.Contains(result.Errors, e => e.PropertyName == "ClosingTime");
    }

    [Fact]
    public void Validate_ShouldFail_WhenMaxOrderNotGreaterThanMin()
    {
        var result = _validator.Validate(RestaurantCommandFactory.CreateRestaurantCommand(
            minOrderAmount: 100m, maxOrderAmount: 100m));

        Assert.Contains(result.Errors, e => e.PropertyName == "MaxOrderAmount");
    }
}
