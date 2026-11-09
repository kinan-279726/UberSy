using UberSy.Application.Features.Restaurants.Menu.Items.Commands.UpdateItemPrice;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Items.UpdateItemPrice;

public sealed class UpdateItemPriceCommandValidatorTest
{
    private readonly UpdateItemPriceCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenValid()
        => Assert.True(_validator.Validate(new UpdateItemPriceCommand(Guid.NewGuid(), 10m)).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenItemIdEmpty()
    {
        var result = _validator.Validate(new UpdateItemPriceCommand(Guid.Empty, 10m));

        Assert.Contains(result.Errors, e => e.PropertyName == "ItemId");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ShouldFail_WhenPriceNotPositive(double price)
    {
        var result = _validator.Validate(new UpdateItemPriceCommand(Guid.NewGuid(), (decimal)price));

        Assert.Contains(result.Errors, e => e.PropertyName == "Price");
    }
}
