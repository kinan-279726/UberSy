using UberSy.Application.Features.ServiceRequests.Commands.PlaceFoodOrder;
using UberSy.Application.Features.ServiceRequests.Queries.GetOrderQuote;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.Queries.GetOrderQuote;

public sealed class GetOrderQuoteQueryValidatorTest
{
    private readonly GetOrderQuoteQueryValidator _validator = new();

    private static GetOrderQuoteQuery Valid() => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        [new PlaceFoodOrderItem(Guid.NewGuid(), 2, [Guid.NewGuid()])]);

    [Fact]
    public void Validate_ShouldPass_WhenValid()
        => Assert.True(_validator.Validate(Valid()).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenRestaurantIdEmpty()
        => Assert.Contains(_validator.Validate(Valid() with { RestaurantId = Guid.Empty }).Errors, e => e.PropertyName == "RestaurantId");

    [Fact]
    public void Validate_ShouldFail_WhenAddressIdEmpty()
        => Assert.Contains(_validator.Validate(Valid() with { AddressId = Guid.Empty }).Errors, e => e.PropertyName == "AddressId");

    [Fact]
    public void Validate_ShouldFail_WhenItemsEmpty()
        => Assert.Contains(_validator.Validate(Valid() with { Items = [] }).Errors, e => e.PropertyName == "Items");

    [Fact]
    public void Validate_ShouldPass_WhenExactlyFiftyLines()
    {
        var items = Enumerable.Range(0, 50).Select(_ => new PlaceFoodOrderItem(Guid.NewGuid(), 1, [])).ToList();

        Assert.True(_validator.Validate(Valid() with { Items = items }).IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenMoreThanFiftyLines()
    {
        var items = Enumerable.Range(0, 51).Select(_ => new PlaceFoodOrderItem(Guid.NewGuid(), 1, [])).ToList();

        var result = _validator.Validate(Valid() with { Items = items });

        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("50"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(100)]
    public void Validate_ShouldFail_WhenQuantityOutOfRange(int quantity)
    {
        var result = _validator.Validate(Valid() with { Items = [new PlaceFoodOrderItem(Guid.NewGuid(), quantity, [])] });

        Assert.Contains(result.Errors, e => e.PropertyName.Contains("Quantity"));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(99)]
    public void Validate_ShouldPass_WhenQuantityIsOnTheBoundary(int quantity)
    {
        var result = _validator.Validate(Valid() with { Items = [new PlaceFoodOrderItem(Guid.NewGuid(), quantity, [])] });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenItemIdEmpty()
    {
        var result = _validator.Validate(Valid() with { Items = [new PlaceFoodOrderItem(Guid.Empty, 1, [])] });

        Assert.Contains(result.Errors, e => e.PropertyName.Contains("ItemId"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenAddOnIdsIsNull()
    {
        var result = _validator.Validate(Valid() with { Items = [new PlaceFoodOrderItem(Guid.NewGuid(), 1, null!)] });

        Assert.Contains(result.Errors, e => e.PropertyName.Contains("AddOnIds"));
    }
}
