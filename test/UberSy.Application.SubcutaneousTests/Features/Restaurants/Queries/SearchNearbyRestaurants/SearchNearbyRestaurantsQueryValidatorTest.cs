using UberSy.Application.Features.Restaurants.Queries.SearchNearbyRestaurants;
using Xunit;
using Assert = Xunit.Assert;

// NOTE: only the validator is tested for this query on purpose. The handler is a cached query whose CacheKey is the
// constant "restaurants" (it ignores latitude / longitude / radius), so two searches from different places would
// return the same cached result. Fix the CacheKey first (e.g. include the rounded coordinates and radius), then
// handler tests can be added without cross-test cache pollution.
namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Queries.SearchNearbyRestaurants;

public sealed class SearchNearbyRestaurantsQueryValidatorTest
{
    private readonly SearchNearbyRestaurantsQueryValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenValid()
        => Assert.True(_validator.Validate(new SearchNearbyRestaurantsQuery(33.5138, 36.2765, 5)).IsValid);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(50.1)]
    public void Validate_ShouldFail_WhenRadiusOutOfRange(double radius)
    {
        var result = _validator.Validate(new SearchNearbyRestaurantsQuery(33.5138, 36.2765, radius));

        Assert.Contains(result.Errors, e => e.PropertyName == "RadiusKm");
    }

    [Fact]
    public void Validate_ShouldPass_WhenRadiusIsMaximum()
        => Assert.True(_validator.Validate(new SearchNearbyRestaurantsQuery(33.5138, 36.2765, 50)).IsValid);

    [Theory]
    [InlineData(90.1)]
    [InlineData(-90.1)]
    public void Validate_ShouldFail_WhenLatitudeOutOfRange(double latitude)
    {
        var result = _validator.Validate(new SearchNearbyRestaurantsQuery(latitude, 36.2765, 5));

        Assert.Contains(result.Errors, e => e.PropertyName == "Latitude");
    }

    [Theory]
    [InlineData(180.1)]
    [InlineData(-180.1)]
    public void Validate_ShouldFail_WhenLongitudeOutOfRange(double longitude)
    {
        var result = _validator.Validate(new SearchNearbyRestaurantsQuery(33.5138, longitude, 5));

        Assert.Contains(result.Errors, e => e.PropertyName == "Longitude");
    }

    [Fact]
    public void Validate_ShouldFail_WhenCoordinatesAreNullIsland()
    {
        var result = _validator.Validate(new SearchNearbyRestaurantsQuery(0, 0, 5));

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(double.NaN, 36.2765)]
    [InlineData(33.5138, double.NaN)]
    [InlineData(double.PositiveInfinity, 36.2765)]
    public void Validate_ShouldFail_WhenCoordinateIsNotANumber(double latitude, double longitude)
    {
        var result = _validator.Validate(new SearchNearbyRestaurantsQuery(latitude, longitude, 5));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldPass_WhenLatitudeIsExactlyZero()
    {
        // KNOWN BUG: NotEmpty() on a double rejects 0, so every point exactly on the equator (or on the
        // prime meridian) is refused. Only the (0, 0) pair should be rejected, which has its own rule.
        var result = _validator.Validate(new SearchNearbyRestaurantsQuery(0, 36.2765, 5));

        Assert.True(result.IsValid);
    }
}
