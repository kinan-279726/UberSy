using UberSy.Application.Features.Restaurants.Menu.Items.Commands.UpdateItemProfile;
using UberSy.Test.Common.Restaurants;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Items.UpdateItemProfile;

public sealed class UpdateItemProfileCommandValidatorTest
{
    private readonly UpdateItemProfileCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenValid()
        => Assert.True(_validator.Validate(ItemCommandFactory.CreateUpdateItemProfileCommand(Guid.NewGuid())).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenItemIdEmpty()
    {
        var result = _validator.Validate(ItemCommandFactory.CreateUpdateItemProfileCommand(Guid.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == "ItemId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_ShouldFail_WhenNameArEmpty(string value)
    {
        var result = _validator.Validate(ItemCommandFactory.CreateUpdateItemProfileCommand(Guid.NewGuid(), nameAr: value));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameAr");
    }

    [Fact]
    public void Validate_ShouldFail_WhenNameEnTooLong()
    {
        var result = _validator.Validate(
            ItemCommandFactory.CreateUpdateItemProfileCommand(Guid.NewGuid(), nameEn: new string('a', 101)));

        Assert.Contains(result.Errors, e => e.PropertyName == "NameEn");
    }

    [Fact]
    public void Validate_ShouldFail_WhenDescriptionArTooLong()
    {
        var result = _validator.Validate(
            ItemCommandFactory.CreateUpdateItemProfileCommand(Guid.NewGuid(), descriptionAr: new string('أ', 1001)));

        Assert.Contains(result.Errors, e => e.PropertyName == "DescriptionAr");
    }

    [Fact]
    public void Validate_ShouldFail_WhenDescriptionEnEmpty()
    {
        var result = _validator.Validate(
            ItemCommandFactory.CreateUpdateItemProfileCommand(Guid.NewGuid(), descriptionEn: ""));

        Assert.Contains(result.Errors, e => e.PropertyName == "DescriptionEn");
    }

    [Fact]
    public void Validate_ShouldFail_WhenImageUrlTooLong()
    {
        var result = _validator.Validate(
            ItemCommandFactory.CreateUpdateItemProfileCommand(Guid.NewGuid(), imageUrl: new string('a', 1001)));

        Assert.Contains(result.Errors, e => e.PropertyName == "ImageUrl");
    }
}
