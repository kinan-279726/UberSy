using UberSy.Application.Features.ServiceRequests.Offers.Commands.AcceptOffer;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.AcceptOffer;

public sealed class AcceptOfferCommandValidatorTest
{
    private readonly AcceptOfferCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenIdIsProvided()
        => Assert.True(_validator.Validate(new AcceptOfferCommand(Guid.NewGuid())).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenIdIsEmpty()
    {
        var result = _validator.Validate(new AcceptOfferCommand(Guid.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == "OfferId");
    }
}
