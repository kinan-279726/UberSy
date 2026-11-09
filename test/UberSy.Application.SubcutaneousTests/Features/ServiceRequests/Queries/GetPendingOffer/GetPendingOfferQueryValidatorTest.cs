using UberSy.Application.Features.ServiceRequests.Queries.GetPendingOffer;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.Queries.GetPendingOffer;

public sealed class GetPendingOfferQueryValidatorTest
{
    private readonly GetPendingOfferQueryValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenIdIsProvided()
        => Assert.True(_validator.Validate(new GetPendingOfferQuery(Guid.NewGuid())).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenIdIsEmpty()
    {
        var result = _validator.Validate(new GetPendingOfferQuery(Guid.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == "OfferId");
    }
}
