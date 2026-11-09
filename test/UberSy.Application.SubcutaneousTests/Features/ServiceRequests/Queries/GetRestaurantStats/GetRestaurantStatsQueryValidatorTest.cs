using UberSy.Application.Features.ServiceRequests.Queries.GetRestaurantStats;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.Queries.GetRestaurantStats;

public sealed class GetRestaurantStatsQueryValidatorTest
{
    private readonly GetRestaurantStatsQueryValidator _validator = new();

    private static readonly DateOnly Start = new(2026, 1, 1);

    [Fact]
    public void Validate_ShouldPass_WhenNoDatesAreGiven()
        => Assert.True(_validator.Validate(new GetRestaurantStatsQuery(Guid.NewGuid(), null, null)).IsValid);

    [Fact]
    public void Validate_ShouldPass_WhenOnlyOneDateIsGiven()
    {
        Assert.True(_validator.Validate(new GetRestaurantStatsQuery(Guid.NewGuid(), Start, null)).IsValid);
        Assert.True(_validator.Validate(new GetRestaurantStatsQuery(Guid.NewGuid(), null, Start)).IsValid);
    }

    [Fact]
    public void Validate_ShouldPass_WhenFromEqualsTo()
        => Assert.True(_validator.Validate(new GetRestaurantStatsQuery(Guid.NewGuid(), Start, Start)).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenRestaurantIdEmpty()
    {
        var result = _validator.Validate(new GetRestaurantStatsQuery(Guid.Empty, null, null));

        Assert.Contains(result.Errors, e => e.PropertyName == "RestaurantId");
    }

    [Fact]
    public void Validate_ShouldFail_WhenFromIsAfterTo()
    {
        var result = _validator.Validate(new GetRestaurantStatsQuery(Guid.NewGuid(), Start.AddDays(1), Start));

        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("From"));
    }

    [Fact]
    public void Validate_ShouldPass_WhenRangeIsExactly93Days()
        => Assert.True(_validator.Validate(new GetRestaurantStatsQuery(Guid.NewGuid(), Start, Start.AddDays(92))).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenRangeExceeds93Days()
    {
        var result = _validator.Validate(new GetRestaurantStatsQuery(Guid.NewGuid(), Start, Start.AddDays(93)));

        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("93"));
    }
}
