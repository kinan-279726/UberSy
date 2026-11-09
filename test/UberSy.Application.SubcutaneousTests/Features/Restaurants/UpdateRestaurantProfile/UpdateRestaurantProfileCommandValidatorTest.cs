using UberSy.Application.Features.Restaurants.Commands.UpdateRestaurantProfile;
using UberSy.Test.Common.Restaurants;
using Xunit;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.UpdateRestaurantProfile;

public sealed class UpdateRestaurantProfileCommandValidatorTest
{
    private readonly UpdateRestaurantProfileCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenValid()
    {
        var result = _validator.Validate(RestaurantCommandFactory.CreateUpdateRestaurantProfileCommand(Guid.NewGuid()));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenRestaurantIdEmpty()
    {
        var result = _validator.Validate(RestaurantCommandFactory.CreateUpdateRestaurantProfileCommand(Guid.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == "RestaurantId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldFail_WhenNameArEmpty(string name)
    {
        var result = _validator.Validate(
            RestaurantCommandFactory.CreateUpdateRestaurantProfileCommand(Guid.NewGuid(), nameAr: name));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameAr");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameArTooLong()
    {
        var result = _validator.Validate(
            RestaurantCommandFactory.CreateUpdateRestaurantProfileCommand(Guid.NewGuid(), nameAr: new string('أ', 101)));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameAr");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldFail_WhenNameEnEmpty(string name)
    {
        var result = _validator.Validate(
            RestaurantCommandFactory.CreateUpdateRestaurantProfileCommand(Guid.NewGuid(), nameEn: name));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameEn");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameEnTooLong()
    {
        var result = _validator.Validate(
            RestaurantCommandFactory.CreateUpdateRestaurantProfileCommand(Guid.NewGuid(), nameEn: new string('a', 101)));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameEn");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPhoneTooLong()
    {
        var result = _validator.Validate(
            RestaurantCommandFactory.CreateUpdateRestaurantProfileCommand(Guid.NewGuid(), phone: new string('1', 21)));

        Assert.Contains(result.Errors, e => e.PropertyName == "Phone");
    }
}
