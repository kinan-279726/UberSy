using UberSy.Application.Features.Drivers.Commands.UpdateDriverLocation;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Drivers.UpdateDriverLocation;

public sealed class UpdateDriverLocationCommandValidatorTest
{
    private readonly UpdateDriverLocationCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenValid()
        => Assert.True(_validator.Validate(new UpdateDriverLocationCommand(Guid.NewGuid(), 33.5, 36.2)).IsValid);

    [Theory]
    [InlineData(-90, -180)]
    [InlineData(90, 180)]
    [InlineData(0, 0)]
    public void Validate_ShouldPass_AtBoundaries(double lat, double lon)
        => Assert.True(_validator.Validate(new UpdateDriverLocationCommand(Guid.NewGuid(), lat, lon)).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenDriverIdEmpty()
    {
        var result = _validator.Validate(new UpdateDriverLocationCommand(Guid.Empty, 33.5, 36.2));
        Assert.Contains(result.Errors, e => e.PropertyName == "DriverId");
    }

    [Theory]
    [InlineData(-90.1)]
    [InlineData(90.1)]
    public void Validate_ShouldFail_WhenLatitudeOutOfRange(double lat)
    {
        var result = _validator.Validate(new UpdateDriverLocationCommand(Guid.NewGuid(), lat, 36.2));
        Assert.Contains(result.Errors, e => e.PropertyName == "Latitude");
    }

    [Theory]
    [InlineData(-180.1)]
    [InlineData(180.1)]
    public void Validate_ShouldFail_WhenLongitudeOutOfRange(double lon)
    {
        var result = _validator.Validate(new UpdateDriverLocationCommand(Guid.NewGuid(), 33.5, lon));
        Assert.Contains(result.Errors, e => e.PropertyName == "Longitude");
    }
}
