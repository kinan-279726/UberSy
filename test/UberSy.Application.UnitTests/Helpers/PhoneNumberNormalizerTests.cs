using UberSy.Application.Common.Helpers;
using Xunit;

namespace UberSy.Application.UnitTests.Helpers;

public sealed class PhoneNumberNormalizerTests
{
    [Theory]
    [InlineData("0933123456", "+963933123456")]
    [InlineData("933123456", "+963933123456")]
    [InlineData("00963933123456", "+963933123456")]
    [InlineData("+963933123456", "+963933123456")]
    [InlineData("+963 933-123-456", "+963933123456")]
    [InlineData("0933 123 456", "+963933123456")]
    public void ToE164_WhenNumberIsValid_ShouldReturnE164(string raw, string expected)
    {
        // Act
        var actual = PhoneNumberNormalizer.ToE164(raw);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("12345")]
    [InlineData("0933123")]
    [InlineData("09331234567")]
    [InlineData("9331234567")]
    public void ToE164_WhenNumberIsInvalid_ShouldReturnNull(string? raw)
    {
        // Act
        var result = PhoneNumberNormalizer.ToE164(raw);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("+963933123456", true)]
    [InlineData("+963 933 123 456", true)]
    [InlineData("+963-933-123-456", true)]
    [InlineData("+96393312345", false)] 
    [InlineData("+9639331234567", false)]
    [InlineData("+12345678", false)]
    [InlineData("+123456789", false)]
    [InlineData("+123456789012345", false)]
    [InlineData("+1234567890123456", false)]
    [InlineData("+44933123456", false)]       
    [InlineData("+971933123456", false)]      
    [InlineData("+963113123456", false)]      
    public void ToE164_WhenInternationalNumber_ShouldOnlyAcceptSyrianMobile(string raw, bool isValid)
    {
        // Act
        var result = PhoneNumberNormalizer.ToE164(raw);

        // Assert
        if (isValid)
        {
            Assert.NotNull(result);
            Assert.StartsWith("+9639", result);
        }
        else
        {
            Assert.Null(result);
        }
    }
}
