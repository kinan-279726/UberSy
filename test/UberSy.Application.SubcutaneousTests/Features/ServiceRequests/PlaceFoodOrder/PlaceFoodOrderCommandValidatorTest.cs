using UberSy.Application.Features.ServiceRequests.Commands.PlaceFoodOrder;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.ServiceRequests.PlaceFoodOrder;

public sealed class PlaceFoodOrderCommandValidatorTest
{
    private readonly PlaceFoodOrderCommandValidator _validator = new();

    private static PlaceFoodOrderCommand Valid() => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        [new PlaceFoodOrderItem(Guid.NewGuid(), 2, [Guid.NewGuid()])]);

    [Fact]
    public void Validate_ShouldPass_WhenValid()
        => Assert.True(_validator.Validate(Valid()).IsValid);

    [Fact]
    public void Validate_ShouldPass_WhenItemHasNoAddOns()
    {
        var command = Valid() with { Items = [new PlaceFoodOrderItem(Guid.NewGuid(), 1, [])] };

        Assert.True(_validator.Validate(command).IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenRestaurantIdEmpty()
    {
        var result = _validator.Validate(Valid() with { RestaurantId = Guid.Empty });

        Assert.Contains(result.Errors, e => e.PropertyName == "RestaurantId");
    }

    [Fact]
    public void Validate_ShouldFail_WhenAddressIdEmpty()
    {
        var result = _validator.Validate(Valid() with { AddressId = Guid.Empty });

        Assert.Contains(result.Errors, e => e.PropertyName == "AddressId");
    }

    [Fact]
    public void Validate_ShouldFail_WhenItemsEmpty()
    {
        var result = _validator.Validate(Valid() with { Items = [] });

        Assert.Contains(result.Errors, e => e.PropertyName == "Items");
    }

    [Fact]
    public void Validate_ShouldFail_WhenItemIdEmpty()
    {
        var result = _validator.Validate(Valid() with { Items = [new PlaceFoodOrderItem(Guid.Empty, 1, [])] });

        Assert.Contains(result.Errors, e => e.PropertyName.Contains("ItemId"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldFail_WhenQuantityIsNotPositive(int quantity)
    {
        var result = _validator.Validate(Valid() with { Items = [new PlaceFoodOrderItem(Guid.NewGuid(), quantity, [])] });

        Assert.Contains(result.Errors, e => e.PropertyName.Contains("Quantity"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenAddOnIdsIsNull()
    {
        // KNOWN BUG: there is no NotNull rule, so a null list passes validation and the calculator then throws a
        // NullReferenceException on requested.AddOnIds.Distinct(). GetOrderQuoteQueryValidator already has this rule.
        var result = _validator.Validate(Valid() with { Items = [new PlaceFoodOrderItem(Guid.NewGuid(), 1, null!)] });

        Assert.Contains(result.Errors, e => e.PropertyName.Contains("AddOnIds"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenQuantityExceedsMaximum()
    {
        // KNOWN GAP: the quote validator limits quantity to 1..99 but the place-order validator has no upper bound,
        // so a customer can get a quote that is rejected and place an order that the quote would have refused.
        var result = _validator.Validate(Valid() with { Items = [new PlaceFoodOrderItem(Guid.NewGuid(), 100, [])] });

        Assert.Contains(result.Errors, e => e.PropertyName.Contains("Quantity"));
    }

    [Fact]
    public void Validate_ShouldFail_WhenMoreThanFiftyLines()
    {
        // KNOWN GAP: same as above for the 50-lines limit that only the quote validator enforces.
        var items = Enumerable.Range(0, 51).Select(_ => new PlaceFoodOrderItem(Guid.NewGuid(), 1, [])).ToList();

        var result = _validator.Validate(Valid() with { Items = items });

        Assert.False(result.IsValid);
    }
}
