using UberSy.Application.Features.Restaurants.Commands.UpdateCommissionRate;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.UpdateCommissionRate;

public sealed class UpdateCommissionRateCommandValidatorTest
{
    private readonly UpdateCommissionRateCommandValidator _validator = new();

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(100)]
    public void Validate_ShouldPass_WhenRateInRange(double rate)
    {
        var result = _validator.Validate(new UpdateCommissionRateCommand(Guid.NewGuid(), (decimal)rate));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenRestaurantIdEmpty()
    {
        var result = _validator.Validate(new UpdateCommissionRateCommand(Guid.Empty, 10m));

        Assert.Contains(result.Errors, e => e.PropertyName == "RestaurantId");
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(100.1)]
    public void Validate_ShouldFail_WhenRateOutOfRange(double rate)
    {
        var result = _validator.Validate(new UpdateCommissionRateCommand(Guid.NewGuid(), (decimal)rate));

        Assert.Contains(result.Errors, e => e.PropertyName == "CommissionRate");
    }
}
