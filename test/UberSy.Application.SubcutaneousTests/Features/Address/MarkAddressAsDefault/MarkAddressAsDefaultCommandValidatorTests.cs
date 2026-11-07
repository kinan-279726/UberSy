using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Address.MarkAddressAsDefault;

public class MarkAddressAsDefaultCommandValidatorTests
{
    private readonly MarkAddressAsDefaultCommandValidator _validator;

    public MarkAddressAsDefaultCommandValidatorTests()
    {
        _validator = new MarkAddressAsDefaultCommandValidator();
    }

    [Fact]
    public void Should_Have_Error_When_AddressId_Is_Empty()
    {
        var command = new MarkAddressAsDefaultCommand(Guid.Empty);
        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "AddressId");
    }

    [Fact]
    public void Should_Pass_When_AddressId_Is_Valid()
    {
        var command = new MarkAddressAsDefaultCommand(Guid.NewGuid());
        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }
}
