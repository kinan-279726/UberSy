using UberSy.Application.Features.ServiceRequests.Commands.RejectFoodOrder;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.RejectOrder;

public sealed class RejectOrderCommandValidatorTest
{
    private readonly RejectOrderCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenValid()
        => Assert.True(_validator.Validate(new RejectOrderCommand(Guid.NewGuid(), "Out of stock")).IsValid);

    [Fact]
    public void Validate_ShouldPass_WhenReasonIsExactlyMaxLength()
        => Assert.True(_validator.Validate(new RejectOrderCommand(Guid.NewGuid(), new string('a', 100))).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenIdEmpty()
    {
        var result = _validator.Validate(new RejectOrderCommand(Guid.Empty, "Out of stock"));

        Assert.Contains(result.Errors, e => e.PropertyName == "ServiceRequestId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_ShouldFail_WhenReasonMissing(string? reason)
    {
        var result = _validator.Validate(new RejectOrderCommand(Guid.NewGuid(), reason!));

        Assert.Contains(result.Errors, e => e.PropertyName == "Reason");
    }

    [Fact]
    public void Validate_ShouldFail_WhenReasonTooLong()
    {
        var result = _validator.Validate(new RejectOrderCommand(Guid.NewGuid(), new string('a', 101)));

        Assert.Contains(result.Errors, e => e.PropertyName == "Reason");
    }
}
