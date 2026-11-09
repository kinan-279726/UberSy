using UberSy.Application.Features.ServiceRequests.Commands.DriverCancelAfterAccepted;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.ReleaseOrder;

public sealed class ReleaseOrderCommandValidatorTest
{
    private readonly ReleaseOrderCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenIdIsProvided()
        => Assert.True(_validator.Validate(new ReleaseOrderCommand(Guid.NewGuid())).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenIdIsEmpty()
    {
        var result = _validator.Validate(new ReleaseOrderCommand(Guid.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == "ServiceRequestId");
    }
}
