using UberSy.Application.Features.Drivers.Commands.TopUpWallet;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.Drivers.TopUpWallet;

public sealed class TopUpWalletCommandValidatorTest
{
    private readonly TopUpWalletCommandValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenValid()
        => Assert.True(_validator.Validate(new TopUpWalletCommand(100m, Guid.NewGuid())).IsValid);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_ShouldFail_WhenAmountNotPositive(double amount)
    {
        var result = _validator.Validate(new TopUpWalletCommand((decimal)amount, Guid.NewGuid()));
        Assert.Contains(result.Errors, e => e.PropertyName == "Amount");
    }

    [Fact]
    public void Validate_ShouldFail_WhenDriverIdEmpty()
    {
        var result = _validator.Validate(new TopUpWalletCommand(50m, Guid.Empty));
        Assert.Contains(result.Errors, e => e.PropertyName == "DriverId");
    }
}
