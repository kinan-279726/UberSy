using UberSy.Application.Features.ServiceRequests.Commands.MarkDelivered;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.MarkDelivered;

public sealed class MarkDeliveredCommandValidatorTest
{
    private readonly MarkDeliveredCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenIdIsProvided()
        => Assert.True(_validator.Validate(new MarkDeliveredCommand(Guid.NewGuid())).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenIdIsEmpty()
    {
        var result = _validator.Validate(new MarkDeliveredCommand(Guid.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == "ServiceRequestId");
    }
}
