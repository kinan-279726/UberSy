using UberSy.Application.Features.Drivers.Commands.ApproveDriver;
using UberSy.Domain.DeliveryRule.Enums;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Drivers.ApproveDriver;

public sealed class ApproveDriverCommandValidatorTest
{
    private readonly ApproveDriverCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenValid()
    {
        var result = _validator.Validate(new ApproveDriverCommand(Guid.NewGuid(), ServiceTier.Standard));
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenDriverIdEmpty()
    {
        var result = _validator.Validate(new ApproveDriverCommand(Guid.Empty, ServiceTier.Standard));
        Assert.Contains(result.Errors, e => e.PropertyName == "DriverId");
    }

    [Fact]
    public void Validate_ShouldFail_WhenServiceTierInvalid()
    {
        var result = _validator.Validate(new ApproveDriverCommand(Guid.NewGuid(), (ServiceTier)111));
        Assert.Contains(result.Errors, e => e.PropertyName == "ServiceTier");
    }
}
