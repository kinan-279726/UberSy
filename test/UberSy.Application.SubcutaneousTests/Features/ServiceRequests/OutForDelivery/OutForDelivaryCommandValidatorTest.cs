using UberSy.Application.Features.ServiceRequests.Commands.OutForDelivery;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.OutForDelivery;

public sealed class OutForDelivaryCommandValidatorTest
{
    private readonly OutForDelivaryCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenIdIsProvided()
        => Assert.True(_validator.Validate(new OutForDelivaryCommand(Guid.NewGuid())).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenIdIsEmpty()
    {
        var result = _validator.Validate(new OutForDelivaryCommand(Guid.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == "ServiceRequestId");
    }
}
