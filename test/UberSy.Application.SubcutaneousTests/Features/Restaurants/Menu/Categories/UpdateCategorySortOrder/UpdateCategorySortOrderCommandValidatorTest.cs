using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.UpdateCategorySortOrder;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Restaurants.Menu.Categories.UpdateCategorySortOrder;

public sealed class UpdateCategorySortOrderCommandValidatorTest
{
    private readonly UpdateCategorySortOrderCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenValid()
        => Assert.True(_validator.Validate(new UpdateCategorySortOrderCommand(Guid.NewGuid(), 5)).IsValid);

    [Fact]
    public void Validate_ShouldFail_WhenCategoryIdEmpty()
    {
        var result = _validator.Validate(new UpdateCategorySortOrderCommand(Guid.Empty, 5));

        Assert.Contains(result.Errors, e => e.PropertyName == "CategoryId");
    }
}
