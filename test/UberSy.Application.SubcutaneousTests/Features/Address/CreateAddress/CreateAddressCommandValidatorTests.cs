using UberSy.Application.Features.Addresses.Commands.CreateAddress;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Address.CreateAddress;

public class CreateAddressCommandValidatorTests
{
    private readonly CreateAddressCommandValidator _validator;

    public CreateAddressCommandValidatorTests()
    {
        _validator = new CreateAddressCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Street_Is_Empty()
    {
        var command = new CreateAddressCommand(
            Street: "",
            City: "Damascus",
            Latitude: 33.5138,
            Longitude: 36.2765,
            Label: "Home",
            Floor: "3",
            Phone: "+963912345678");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Street");
    }

    [Fact]
    public void Should_Have_Error_When_Street_Exceeds_Max_Length()
    {
        var longStreet = new string('A', 101); 

        var command = new CreateAddressCommand(
            Street: longStreet,
            City: "Damascus",
            Latitude: 33.5138,
            Longitude: 36.2765,
            Label: "Home",
            Floor: "3",
            Phone: "+963912345678");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Street");
    }

    [Fact]
    public void Should_Have_Error_When_City_Is_Empty()
    {
        var command = new CreateAddressCommand(
            Street: "King Faisal Street",
            City: "",
            Latitude: 33.5138,
            Longitude: 36.2765,
            Label: "Home",
            Floor: "3",
            Phone: "+963912345678");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "City");
    }

    [Fact]
    public void Should_Have_Error_When_City_Exceeds_Max_Length()
    {
        var longCity = new string('A', 101);

        var command = new CreateAddressCommand(
            Street: "King Faisal Street",
            City: longCity,
            Latitude: 33.5138,
            Longitude: 36.2765,
            Label: "Home",
            Floor: "3",
            Phone: "+963912345678");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "City");
    }

    [Theory]
    [InlineData(-90.1)]
    [InlineData(90.1)]
    public void Should_Have_Error_When_Latitude_Is_Out_Of_Range(double invalidLatitude)
    {
        var command = new CreateAddressCommand(
            Street: "King Faisal Street",
            City: "Damascus",
            Latitude: invalidLatitude,
            Longitude: 36.2765,
            Label: "Home",
            Floor: "3",
            Phone: "+963912345678");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Latitude");
    }

    [Theory]
    [InlineData(-180.1)]
    [InlineData(180.1)]
    public void Should_Have_Error_When_Longitude_Is_Out_Of_Range(double invalidLongitude)
    {
        var command = new CreateAddressCommand(
            Street: "King Faisal Street",
            City: "Damascus",
            Latitude: 33.5138,
            Longitude: invalidLongitude,
            Label: "Home",
            Floor: "3",
            Phone: "+963912345678");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Longitude");
    }

    [Fact]
    public void Should_Have_Error_When_Label_Exceeds_Max_Length()
    {
        var longLabel = new string('A', 101);

        var command = new CreateAddressCommand(
            Street: "King Faisal Street",
            City: "Damascus",
            Latitude: 33.5138,
            Longitude: 36.2765,
            Label: longLabel,
            Floor: "3",
            Phone: "+963912345678");

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Label");
    }

    [Fact]
    public void Should_Have_Error_When_Phone_Exceeds_Max_Length()
    {
        var longPhone = new string('1', 21);

        var command = new CreateAddressCommand(
            Street: "King Faisal Street",
            City: "Damascus",
            Latitude: 33.5138,
            Longitude: 36.2765,
            Label: "Home",
            Floor: "3",
            Phone: longPhone);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Phone");
    }

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var command = new CreateAddressCommand(
            Street: "King Faisal Street",
            City: "Damascus",
            Latitude: 33.5138,
            Longitude: 36.2765,
            Label: "Home",
            Floor: "3",
            Phone: "+963912345678");

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
