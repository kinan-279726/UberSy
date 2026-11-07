using UberSy.Application.Features.DeliveryPricingRules.Commands.CreatePricingRule;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Test.Common.DeliveryPricingRule;
using Xunit;
using Assert = Xunit.Assert;

namespace UberSy.Application.SubcutaneousTests.Features.DeliveryPricingRules.CreatePricingRule;

public class CreatePricingRuleCommandValidatorTests
{
    private readonly CreatePricingRuleCommandValidator _validator = new();

    [Fact]
    public void Should_Pass_When_Valid()
    {
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand();

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_Pass_When_Vehicle_Is_Car_With_CarType()
    {
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            vehicleType: VehicleType.Car,
            carType: CarType.Taxi);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Should_Have_Error_When_VehicleType_Is_Not_Defined()
    {
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(vehicleType: (VehicleType)99);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "VehicleType");
    }

    [Fact]
    public void Should_Have_Error_When_ServiceTier_Is_Not_Defined()
    {
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(serviceTier: (ServiceTier)99);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ServiceTier");
    }

    [Fact]
    public void Should_Have_Error_When_CarType_Is_Not_Defined()
    {
        // The factory only keeps CarType when the vehicle is a Car
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            vehicleType: VehicleType.Car,
            carType: (CarType)99);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CarType");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_CustomerRatePerKm_Is_Not_Positive(double rate)
    {
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            customerRatePerKm: (decimal)rate);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "CustomerRatePerKm");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_DriverRatePerKm_Is_Not_Positive(double rate)
    {
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            driverRatePerKm: (decimal)rate);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "DriverRatePerKm");
    }

    [Fact]
    public void Should_Have_Error_When_DriverRatePerKm_Exceeds_CustomerRatePerKm()
    {
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            customerRatePerKm: 5m,
            driverRatePerKm: 6m);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "DriverRatePerKm");
    }

    [Fact]
    public void Should_Pass_When_DriverRatePerKm_Equals_CustomerRatePerKm()
    {
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            customerRatePerKm: 5m,
            driverRatePerKm: 5m);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_Have_Error_When_DriverRatePerOrder_Is_Set_And_Not_Positive(double value)
    {
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            driverRatePerOrder: (decimal)value);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "DriverRatePerOrder");
    }

    [Fact]
    public void Should_Pass_When_DriverRatePerOrder_Is_Positive()
    {
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(driverRatePerOrder: 2m);

        Assert.True(_validator.Validate(command).IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Should_Have_Error_When_MaxDistanceKm_Is_Set_And_Not_Positive(double value)
    {
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            maxDistanceKm: (decimal)value);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MaxDistanceKm");
    }

    [Fact]
    public void Should_Pass_When_MaxDistanceKm_Is_Positive()
    {
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(maxDistanceKm: 15m);

        Assert.True(_validator.Validate(command).IsValid);
    }

    [Fact]
    public void Should_Have_Error_When_Only_StartHour_Is_Set()
    {
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            startHour: new TimeOnly(8, 0),
            endHour: null);

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("StartHour and EndHour"));
    }

    [Fact]
    public void Should_Have_Error_When_Only_EndHour_Is_Set()
    {
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            startHour: null,
            endHour: new TimeOnly(20, 0));

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("StartHour and EndHour"));
    }

    [Fact]
    public void Should_Have_Error_When_EndHour_Is_Before_StartHour()
    {
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            startHour: new TimeOnly(20, 0),
            endHour: new TimeOnly(8, 0));

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EndHour");
    }

    [Fact]
    public void Should_Have_Error_When_EndHour_Equals_StartHour()
    {
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            startHour: new TimeOnly(8, 0),
            endHour: new TimeOnly(8, 0));

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EndHour");
    }

    [Fact]
    public void Should_Pass_When_EndHour_Is_After_StartHour()
    {
        var command = PricingRuleCommandFactory.CreateCreatePricingRuleCommand(
            startHour: new TimeOnly(8, 0),
            endHour: new TimeOnly(20, 0));

        Assert.True(_validator.Validate(command).IsValid);
    }
}
