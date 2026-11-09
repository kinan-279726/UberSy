using UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.CreateAddOn;
using UberSy.Test.Common.Restaurants;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.AddOns.CreateAddOn;

public sealed class CreateAddOnCommandValidatorTest
{
    private readonly CreateAddOnCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenValid()
        => Assert.True(_validator.Validate(AddOnCommandFactory.CreateCreateAddOnCommand(Guid.NewGuid())).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenRestaurantIdEmpty()
    {
        var result = _validator.Validate(AddOnCommandFactory.CreateCreateAddOnCommand(Guid.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == "RestaurantId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldFail_WhenNameArEmpty(string name)
    {
        var result = _validator.Validate(AddOnCommandFactory.CreateCreateAddOnCommand(Guid.NewGuid(), nameAr: name));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameAr");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameEnTooLong()
    {
        var result = _validator.Validate(
            AddOnCommandFactory.CreateCreateAddOnCommand(Guid.NewGuid(), nameEn: new string('a', 101)));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameEn");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldFail_WhenPriceNotPositive(double price)
    {
        var result = _validator.Validate(
            AddOnCommandFactory.CreateCreateAddOnCommand(Guid.NewGuid(), price: (decimal)price));

        Assert.Contains(result.Errors, e => e.PropertyName == "Price");
    }
}
