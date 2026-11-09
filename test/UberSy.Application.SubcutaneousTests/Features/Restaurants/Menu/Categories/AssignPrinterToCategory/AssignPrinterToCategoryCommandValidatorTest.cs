using UberSy.Test.Common.Restaurants;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Categories.AssignPrinterToCategory;

// Note: AssignPrinterToCategoryCommand and its validator live in the global namespace
public sealed class AssignPrinterToCategoryCommandValidatorTest
{
    private readonly AssignPrinterToCategoryCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenValid()
    {
        var result = _validator.Validate(
            CategoryCommandFactory.CreateAssignPrinterToCategoryCommand(Guid.NewGuid(), Guid.NewGuid()));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_ShouldFail_WhenCategoryIdEmpty()
    {
        var result = _validator.Validate(new AssignPrinterToCategoryCommand(Guid.Empty, Guid.NewGuid()));

        Assert.Contains(result.Errors, e => e.PropertyName == "CategoryId");
    }

    [Fact]
    public void Validate_ShouldFail_WhenPrinterIdEmpty()
    {
        var result = _validator.Validate(new AssignPrinterToCategoryCommand(Guid.NewGuid(), Guid.Empty));

        Assert.Contains(result.Errors, e => e.PropertyName == "PrinterId");
    }
}
