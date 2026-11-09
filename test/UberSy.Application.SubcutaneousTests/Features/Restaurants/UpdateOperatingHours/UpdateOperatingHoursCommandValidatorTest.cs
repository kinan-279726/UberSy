using UberSy.Application.Features.Restaurants.Commands.UpdateOperatingHours;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.UpdateOperatingHours;

public sealed class UpdateOperatingHoursCommandValidatorTest
{
    private readonly UpdateOperatingHoursCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenValid()
    {
        var result = _validator.Validate(
            new UpdateOperatingHoursCommand(Guid.NewGuid(), new TimeOnly(8, 0), new TimeOnly(23, 0)));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenRestaurantIdEmpty()
    {
        var result = _validator.Validate(
            new UpdateOperatingHoursCommand(Guid.Empty, new TimeOnly(8, 0), new TimeOnly(23, 0)));

        Assert.Contains(result.Errors, e => e.PropertyName == "RestaurantId");
    }

    [Theory]
    [InlineData(10, 10)]
    [InlineData(22, 9)]
    public void Validate_ShouldFail_WhenClosingNotAfterOpening(int openHour, int closeHour)
    {
        var result = _validator.Validate(
            new UpdateOperatingHoursCommand(Guid.NewGuid(), new TimeOnly(openHour, 0), new TimeOnly(closeHour, 0)));

        Assert.Contains(result.Errors, e => e.PropertyName == "ClosingTime");
    }
}
