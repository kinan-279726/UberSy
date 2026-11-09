using UberSy.Application.Features.Restaurants.Commands.UpdateTaxRate;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.UpdateTaxRate;

public sealed class UpdateTaxRateCommandValidatorTest
{
    private readonly UpdateTaxRateCommandValidator _validator = new();

    [Theory]
    [InlineData(5)]
    [InlineData(100)]
    public void Validate_ShouldPass_WhenRateInRange(double rate)
    {
        var result = _validator.Validate(new UpdateTaxRateCommand(Guid.NewGuid(), (decimal)rate));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldPass_WhenRateIsZero()
    {
        // KNOWN BUG: the validator uses NotEmpty() on a decimal, which rejects 0, so a 0% tax cannot be set.
        var result = _validator.Validate(new UpdateTaxRateCommand(Guid.NewGuid(), 0m));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenRestaurantIdEmpty()
    {
        var result = _validator.Validate(new UpdateTaxRateCommand(Guid.Empty, 5m));

        Assert.Contains(result.Errors, e => e.PropertyName == "RestaurantId");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(100.1)]
    public void Validate_ShouldFail_WhenRateOutOfRange(double rate)
    {
        var result = _validator.Validate(new UpdateTaxRateCommand(Guid.NewGuid(), (decimal)rate));

        Assert.Contains(result.Errors, e => e.PropertyName == "TaxRate");
    }
}
