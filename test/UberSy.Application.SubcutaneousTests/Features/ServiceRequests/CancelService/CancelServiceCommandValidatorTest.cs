using UberSy.Application.Features.ServiceRequests.Commands.CancelService;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.CancelService;

public sealed class CancelServiceCommandValidatorTest
{
    private readonly CancelServiceCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenIdIsProvided()
        => Assert.True(_validator.Validate(new CancelServiceCommand(Guid.NewGuid())).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenIdIsEmpty()
    {
        var result = _validator.Validate(new CancelServiceCommand(Guid.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == "ServiceId");
    }
}
