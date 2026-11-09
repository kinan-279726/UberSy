using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.UpdateCategoryProfile;
using UberSy.Test.Common.Restaurants;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Categories.UpdateCategoryProfile;

public sealed class UpdateCategoryProfileCommandValidatorTest
{
    private readonly UpdateCategoryProfileCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenValid()
    {
        var result = _validator.Validate(CategoryCommandFactory.CreateUpdateCategoryProfileCommand(Guid.NewGuid()));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenCategoryIdEmpty()
    {
        var result = _validator.Validate(CategoryCommandFactory.CreateUpdateCategoryProfileCommand(Guid.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == "CategoryId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldFail_WhenNameArEmpty(string name)
    {
        var result = _validator.Validate(
            CategoryCommandFactory.CreateUpdateCategoryProfileCommand(Guid.NewGuid(), nameAr: name));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameAr");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameArTooLong()
    {
        var result = _validator.Validate(
            CategoryCommandFactory.CreateUpdateCategoryProfileCommand(Guid.NewGuid(), nameAr: new string('أ', 101)));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameAr");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldFail_WhenNameEnEmpty(string name)
    {
        var result = _validator.Validate(
            CategoryCommandFactory.CreateUpdateCategoryProfileCommand(Guid.NewGuid(), nameEn: name));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameEn");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameEnTooLong()
    {
        var result = _validator.Validate(
            CategoryCommandFactory.CreateUpdateCategoryProfileCommand(Guid.NewGuid(), nameEn: new string('a', 101)));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameEn");
    }
}
