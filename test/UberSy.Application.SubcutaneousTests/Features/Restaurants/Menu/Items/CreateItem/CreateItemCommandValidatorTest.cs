using UberSy.Application.Features.Restaurants.Menu.Items.Commands.CreateItem;
using UberSy.Test.Common.Restaurants;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Items.CreateItem;

public sealed class CreateItemCommandValidatorTest
{
    private readonly CreateItemCommandValidator _validator = new();


    [Fact]
    public void Validate_ShouldPass_WhenValid()
    {
        var result = _validator.Validate(ItemCommandFactory.CreateCreateItemCommand(Guid.NewGuid(), Guid.NewGuid()));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenRestaurantIdEmpty()
    {
        var result = _validator.Validate(ItemCommandFactory.CreateCreateItemCommand(Guid.Empty, Guid.NewGuid()));

        Assert.Contains(result.Errors, e => e.PropertyName == "RestaurantId");
    }

    [Fact]
    public void Validate_ShouldFail_WhenCategoryIdEmpty()
    {
        var result = _validator.Validate(ItemCommandFactory.CreateCreateItemCommand(Guid.NewGuid(), Guid.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == "CategoryId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldFail_WhenNameArEmpty(string name)
    {
        var result = _validator.Validate(
            ItemCommandFactory.CreateCreateItemCommand(Guid.NewGuid(), Guid.NewGuid(), nameAr: name));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameAr");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameEnTooLong()
    {
        var result = _validator.Validate(
            ItemCommandFactory.CreateCreateItemCommand(Guid.NewGuid(), Guid.NewGuid(), nameEn: new string('a', 101)));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameEn");
    }

    [Fact]
    public void Validate_ShouldFail_WhenDescriptionArTooLong()
    {
        var result = _validator.Validate(
            ItemCommandFactory.CreateCreateItemCommand(Guid.NewGuid(), Guid.NewGuid(), descriptionAr: new string('أ', 1001)));

        Assert.Contains(result.Errors, e => e.PropertyName == "DescriptionAr");
    }

    [Fact]
    public void Validate_ShouldFail_WhenDescriptionEnEmpty()
    {
        var result = _validator.Validate(
            ItemCommandFactory.CreateCreateItemCommand(Guid.NewGuid(), Guid.NewGuid(), descriptionEn: ""));

        Assert.Contains(result.Errors, e => e.PropertyName == "DescriptionEn");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Validate_ShouldFail_WhenPriceNotPositive(double price)
    {
        var result = _validator.Validate(
            ItemCommandFactory.CreateCreateItemCommand(Guid.NewGuid(), Guid.NewGuid(), price: (decimal)price));

        Assert.Contains(result.Errors, e => e.PropertyName == "Price");
    }

    [Fact]
    public void Validate_ShouldFail_WhenImageUrlEmpty()
    {
        var result = _validator.Validate(
            ItemCommandFactory.CreateCreateItemCommand(Guid.NewGuid(), Guid.NewGuid(), imageUrl: " "));

        Assert.Contains(result.Errors, e => e.PropertyName == "ImageUrl");
    }

    [Fact]
    public void Validate_ShouldFail_WhenImageUrlTooLong()
    {
        var result = _validator.Validate(
            ItemCommandFactory.CreateCreateItemCommand(Guid.NewGuid(), Guid.NewGuid(), imageUrl: new string('a', 1001)));

        Assert.Contains(result.Errors, e => e.PropertyName == "ImageUrl");
    }

    [Fact]
    public void Validate_ShouldFail_WhenCaloriesNegative()
    {
        var result = _validator.Validate(
            ItemCommandFactory.CreateCreateItemCommand(Guid.NewGuid(), Guid.NewGuid(), calories: -1));

        Assert.Contains(result.Errors, e => e.PropertyName == "Calories");
    }

    [Fact]
    public void Validate_ShouldPass_WhenCaloriesZero()
    {
        var result = _validator.Validate(
            ItemCommandFactory.CreateCreateItemCommand(Guid.NewGuid(), Guid.NewGuid(), calories: 0));

        Assert.True(result.IsValid);
    }

}
