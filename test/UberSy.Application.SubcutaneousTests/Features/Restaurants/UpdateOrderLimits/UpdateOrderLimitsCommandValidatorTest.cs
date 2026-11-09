using UberSy.Application.Features.Restaurants.Commands.UpdateOrderLimits;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.UpdateOrderLimits;

public sealed class UpdateOrderLimitsCommandValidatorTest
{
    private readonly UpdateOrderLimitsCommandValidator _validator = new();

    [Theory]
    [InlineData(0, 100)]
    [InlineData(10, 500)]
    public void Validate_ShouldPass_WhenValid(double min, double max)
    {
        var result = _validator.Validate(new UpdateOrderLimitsCommand(Guid.NewGuid(), (decimal)min, (decimal)max));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenRestaurantIdEmpty()
    {
        var result = _validator.Validate(new UpdateOrderLimitsCommand(Guid.Empty, 10m, 100m));

        Assert.Contains(result.Errors, e => e.PropertyName == "RestaurantId");
    }

    [Fact]
    public void Validate_ShouldFail_WhenMinNegative()
    {
        var result = _validator.Validate(new UpdateOrderLimitsCommand(Guid.NewGuid(), -1m, 100m));

        Assert.Contains(result.Errors, e => e.PropertyName == "MinOrderAmount");
    }

    [Theory]
    [InlineData(100, 100)]
    [InlineData(100, 50)]
    public void Validate_ShouldFail_WhenMaxNotGreaterThanMin(double min, double max)
    {
        var result = _validator.Validate(new UpdateOrderLimitsCommand(Guid.NewGuid(), (decimal)min, (decimal)max));

        Assert.Contains(result.Errors, e => e.PropertyName == "MaxOrderAmount");
    }
}
