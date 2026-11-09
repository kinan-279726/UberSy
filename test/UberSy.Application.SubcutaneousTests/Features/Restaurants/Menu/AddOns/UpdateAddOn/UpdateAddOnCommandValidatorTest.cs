using UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.UpdateAddOn;
using UberSy.Test.Common.Restaurants;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.AddOns.UpdateAddOn;

public sealed class UpdateAddOnCommandValidatorTest
{
    private readonly UpdateAddOnCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenValid()
        => Assert.True(_validator.Validate(AddOnCommandFactory.CreateUpdateAddOnCommand(Guid.NewGuid())).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenAddOnIdEmpty()
    {
        var result = _validator.Validate(AddOnCommandFactory.CreateUpdateAddOnCommand(Guid.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == "AddOnId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldFail_WhenNameEnEmpty(string name)
    {
        var result = _validator.Validate(AddOnCommandFactory.CreateUpdateAddOnCommand(Guid.NewGuid(), nameEn: name));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameEn");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameArTooLong()
    {
        var result = _validator.Validate(
            AddOnCommandFactory.CreateUpdateAddOnCommand(Guid.NewGuid(), nameAr: new string('أ', 101)));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameAr");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void Validate_ShouldFail_WhenPriceNotPositive(double price)
    {
        var result = _validator.Validate(
            AddOnCommandFactory.CreateUpdateAddOnCommand(Guid.NewGuid(), price: (decimal)price));

        Assert.Contains(result.Errors, e => e.PropertyName == "Price");
    }
}
