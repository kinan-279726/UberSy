using UberSy.Domain.Tags;
using Xunit;

namespace UberSy.Domain.UnitTests.Tags;

public class TagTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbTag CreateValidTag()
    {
        return TbTag.Create(
            id: Guid.NewGuid(),
            nameAr: "حلال",
            nameEn: "Halal"
        ).Value;
    }

    // ============================================================
    // Create Tests
    // ============================================================

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_ShouldReturnError_WhenNameArIsInvalid(string invalidName)
    {
        var result = TbTag.Create(
            id: Guid.NewGuid(),
            nameAr: invalidName!,
            nameEn: "Halal");

        Assert.False(result.IsSuccess);
        Assert.Equal(TagErrors.NameRequired.Code, result.TopError.Code);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_ShouldReturnError_WhenNameEnIsInvalid(string invalidName)
    {
        var result = TbTag.Create(
            id: Guid.NewGuid(),
            nameAr: "حلال",
            nameEn: invalidName!);

        Assert.False(result.IsSuccess);
        Assert.Equal(TagErrors.NameRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var nameAr = "  حلال  ";
        var nameEn = "  Halal  ";

        // Act
        var result = TbTag.Create(id, nameAr, nameEn);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(id, result.Value.Id);
        Assert.Equal("حلال", result.Value.NameAr); // Verify Trim
        Assert.Equal("Halal", result.Value.NameEn); // Verify Trim
    }

    // ============================================================
    // Update Tests
    // ============================================================

    [Theory]
    [InlineData("", "Valid")]
    [InlineData("   ", "Valid")]
    [InlineData(null, "Valid")]
    [InlineData("Valid", "")]
    [InlineData("Valid", "   ")]
    [InlineData("Valid", null)]
    public void Update_ShouldReturnError_WhenAnyNameIsInvalid(string invalidNameAr, string invalidNameEn)
    {
        var tag = CreateValidTag();

        var result = tag.Update(invalidNameAr!, invalidNameEn!);

        Assert.False(result.IsSuccess);
        Assert.Equal(TagErrors.NameRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Update_ShouldReturnSuccess_AndUpdateProperties()
    {
        // Arrange
        var tag = CreateValidTag();
        Assert.Equal("حلال", tag.NameAr);
        Assert.Equal("Halal", tag.NameEn);

        var newNameAr = "  نباتي  ";
        var newNameEn = "  Vegan  ";

        // Act
        var result = tag.Update(newNameAr, newNameEn);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("نباتي", tag.NameAr); // Verify Trim
        Assert.Equal("Vegan", tag.NameEn); // Verify Trim
    }
}
