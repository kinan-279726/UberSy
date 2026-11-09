using UberSy.Application.Features.Drivers.Commands.RegisterDriver;
using UberSy.Domain.DeliveryRule.Enums;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Drivers.RegisterDriver;

public sealed class RegisterDriverCommandValidatorTest
{
    private readonly RegisterDriverCommandValidator _validator = new();

    private static RegisterDriverCommand Valid(
        VehicleType vehicleType = VehicleType.Motorcycle,
        CarType? carType = null,
        string nationalId = "123456789",
        string? photoUrl = null,
        string? license = "LIC-1",
        string? plate = "PLATE-1",
        string? model = "Honda",
        string? color = "Red")
        => new(vehicleType, carType, nationalId, photoUrl, license, plate, model, color);

    [Fact]
    public void Validate_ShouldPass_WhenValid()
        => Assert.True(_validator.Validate(Valid()).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenVehicleTypeInvalid()
    {
        var result = _validator.Validate(Valid(vehicleType: (VehicleType)111));
        Assert.Contains(result.Errors, e => e.PropertyName == "VehicleType");
    }

    [Fact]
    public void Validate_ShouldFail_WhenCarTypeInvalid()
    {
        var result = _validator.Validate(Valid(carType: (CarType)111));
        Assert.Contains(result.Errors, e => e.PropertyName == "CarType");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldFail_WhenNationalIdEmpty(string nationalId)
    {
        var result = _validator.Validate(Valid(nationalId: nationalId));
        Assert.Contains(result.Errors, e => e.PropertyName == "NationalId");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNationalIdTooLong()
    {
        var result = _validator.Validate(Valid(nationalId: new string('1', 21)));
        Assert.Contains(result.Errors, e => e.PropertyName == "NationalId");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPhotoUrlTooLong()
    {
        var result = _validator.Validate(Valid(photoUrl: new string('a', 1001)));
        Assert.Contains(result.Errors, e => e.PropertyName == "PhotoUrl");
    }

    [Fact]
    public void Validate_ShouldFail_WhenLicenseTooLong()
    {
        var result = _validator.Validate(Valid(license: new string('a', 51)));
        Assert.Contains(result.Errors, e => e.PropertyName == "LicenseNumber");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPlateTooLong()
    {
        var result = _validator.Validate(Valid(plate: new string('a', 21)));
        Assert.Contains(result.Errors, e => e.PropertyName == "PlateNumber");
    }

    [Fact]
    public void Validate_ShouldFail_WhenModelTooLong()
    {
        var result = _validator.Validate(Valid(model: new string('a', 101)));
        Assert.Contains(result.Errors, e => e.PropertyName == "Model");
    }

    [Fact]
    public void Validate_ShouldFail_WhenColorTooLong()
    {
        var result = _validator.Validate(Valid(color: new string('a', 101)));
        Assert.Contains(result.Errors, e => e.PropertyName == "Color");
    }
}
