using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.CreateCategory;
using UberSy.Test.Common.Restaurants;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Categories.CreateCategory;

public sealed class CreateCategoryCommandValidatorTest
{
    private readonly CreateCategoryCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenValid()
    {
        var result = _validator.Validate(CategoryCommandFactory.CreateCreateCategoryCommand(Guid.NewGuid()));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenRestaurantIdEmpty()
    {
        var result = _validator.Validate(CategoryCommandFactory.CreateCreateCategoryCommand(Guid.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == "RestaurantId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldFail_WhenNameArEmpty(string name)
    {
        var result = _validator.Validate(
            CategoryCommandFactory.CreateCreateCategoryCommand(Guid.NewGuid(), nameAr: name));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameAr");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameArTooLong()
    {
        var result = _validator.Validate(
            CategoryCommandFactory.CreateCreateCategoryCommand(Guid.NewGuid(), nameAr: new string('أ', 101)));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameAr");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldFail_WhenNameEnEmpty(string name)
    {
        var result = _validator.Validate(
            CategoryCommandFactory.CreateCreateCategoryCommand(Guid.NewGuid(), nameEn: name));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameEn");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameEnTooLong()
    {
        var result = _validator.Validate(
            CategoryCommandFactory.CreateCreateCategoryCommand(Guid.NewGuid(), nameEn: new string('a', 101)));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameEn");
    }
}
