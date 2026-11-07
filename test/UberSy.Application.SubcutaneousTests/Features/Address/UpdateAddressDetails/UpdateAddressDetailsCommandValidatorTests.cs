using UberSy.Application.Features.Addresses.Commands.UpdateAddressDetails;
using UberSy.Application.SubcutaneousTests.Common.Factories;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Address.UpdateAddressDetails;

public class UpdateAddressDetailsCommandValidatorTests
{
    private readonly UpdateAddressDetailsCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var command = AddressCommandFactory.CreateUpdateAddressDetailsCommand();

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_Have_Error_When_AddressId_Is_Empty()
    {
        var command = AddressCommandFactory.CreateUpdateAddressDetailsCommand(addressId: Guid.Empty);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "AddressId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Have_Error_When_Street_Is_Empty(string street)
    {
        var command = AddressCommandFactory.CreateUpdateAddressDetailsCommand(street: street);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Street");
    }

    [Fact]
    public void Should_Have_Error_When_Street_Exceeds_Max_Length()
    {
        var command = AddressCommandFactory.CreateUpdateAddressDetailsCommand(street: new string('A', 101));

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Street");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_Have_Error_When_City_Is_Empty(string city)
    {
        var command = AddressCommandFactory.CreateUpdateAddressDetailsCommand(city: city);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "City");
    }

    [Fact]
    public void Should_Have_Error_When_City_Exceeds_Max_Length()
    {
        var command = AddressCommandFactory.CreateUpdateAddressDetailsCommand(city: new string('A', 101));

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "City");
    }

    [Theory]
    [InlineData(-90.1)]
    [InlineData(90.1)]
    public void Should_Have_Error_When_Latitude_Is_Out_Of_Range(double latitude)
    {
        var command = AddressCommandFactory.CreateUpdateAddressDetailsCommand(latitude: latitude);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Latitude");
    }

    [Theory]
    [InlineData(-180.1)]
    [InlineData(180.1)]
    public void Should_Have_Error_When_Longitude_Is_Out_Of_Range(double longitude)
    {
        var command = AddressCommandFactory.CreateUpdateAddressDetailsCommand(longitude: longitude);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Longitude");
    }

    [Theory]
    [InlineData(-90)]
    [InlineData(90)]
    public void Should_Pass_When_Latitude_Is_At_Boundary(double latitude)
    {
        var command = AddressCommandFactory.CreateUpdateAddressDetailsCommand(latitude: latitude);

        Assert.True(_validator.Validate(command).IsValid);
    }

    [Theory]
    [InlineData(-180)]
    [InlineData(180)]
    public void Should_Pass_When_Longitude_Is_At_Boundary(double longitude)
    {
        var command = AddressCommandFactory.CreateUpdateAddressDetailsCommand(longitude: longitude);

        Assert.True(_validator.Validate(command).IsValid);
    }
}
