using UberSy.Application.Features.ServiceRequests.Commands.AcceptFoodOrder;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.AcceptOrder;

public sealed class AcceptOrderCommandValidatorTest
{
    private readonly AcceptOrderCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenIdIsProvided()
        => Assert.True(_validator.Validate(new AcceptOrderCommand(Guid.NewGuid())).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenIdIsEmpty()
    {
        var result = _validator.Validate(new AcceptOrderCommand(Guid.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == "ServiceRequestId");
    }
}
