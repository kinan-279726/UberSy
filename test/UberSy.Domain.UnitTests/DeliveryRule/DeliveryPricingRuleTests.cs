using UberSy.Domain.DeliveryRule;
using UberSy.Domain.DeliveryRule.Enums;
using Xunit;

namespace UberSy.Domain.UnitTests.DeliveryRule;

public class DeliveryPricingRuleTests
{
    // ============================================================
    // Helper Methods
    // ============================================================
    private static TbDeliveryPricingRule CreateValidRule()
    {
        return TbDeliveryPricingRule.Create(
            id: Guid.NewGuid(),
            vehicleType: VehicleType.Motorcycle,
            serviceTier: ServiceTier.Standard,
            carType: null,
            customerRatePerKm: 10m,
            driverRatePerKm: 8m,
            driverRatePerOrder: 5m,
            maxDistanceKm: 50m,
            startHour: new TimeOnly(8, 0),
            endHour: new TimeOnly(20, 0)
        ).Value;
    }

    // ============================================================
    // Create Tests - Configuration Validation
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenVehicleTypeIsInvalid()
    {
        var result = TbDeliveryPricingRule.Create(
            Guid.NewGuid(), (VehicleType)222, ServiceTier.Standard, null, 10m, 8m, null, null, null, null);

        Assert.False(result.IsSuccess);
        Assert.Equal(DeliveryRuleErrors.VehicleTypeInvalid.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenServiceTierIsInvalid()
    {
        var result = TbDeliveryPricingRule.Create(
            Guid.NewGuid(), VehicleType.Motorcycle, (ServiceTier)222, null, 10m, 8m, null, null, null, null);

        Assert.False(result.IsSuccess);
        Assert.Equal(DeliveryRuleErrors.ServiceTierInvalid.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenPremiumTierIsNotCar()
    {
        var result = TbDeliveryPricingRule.Create(
            Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Premium, null, 10m, 8m, null, null, null, null);

        Assert.False(result.IsSuccess);
        Assert.Equal(DeliveryRuleErrors.ServiceTierRequiresCar.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenCarTypeIsMissingForCar()
    {
        var result = TbDeliveryPricingRule.Create(
            Guid.NewGuid(), VehicleType.Car, ServiceTier.Standard, null, 10m, 8m, null, null, null, null);

        Assert.False(result.IsSuccess);
        Assert.Equal(DeliveryRuleErrors.CarTypeRequired.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenCarTypeIsProvidedForNonCar()
    {
        var result = TbDeliveryPricingRule.Create(
            Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Standard, CarType.Taxi, 10m, 8m, null, null, null, null);

        Assert.False(result.IsSuccess);
        Assert.Equal(DeliveryRuleErrors.CarTypeNotAllowed.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenCarTypeIsInvalid()
    {
        var result = TbDeliveryPricingRule.Create(
            Guid.NewGuid(), VehicleType.Car, ServiceTier.Standard, (CarType)222, 10m, 8m, null, null, null, null);

        Assert.False(result.IsSuccess);
        Assert.Equal(DeliveryRuleErrors.CarTypeInvalid.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenPremiumCarIsNotPrivate()
    {
        var result = TbDeliveryPricingRule.Create(
            Guid.NewGuid(), VehicleType.Car, ServiceTier.Premium, CarType.Taxi, 10m, 8m, null, null, null, null);

        Assert.False(result.IsSuccess);
        Assert.Equal(DeliveryRuleErrors.CarTypePremiumRequiresPrivate.Code, result.TopError.Code);
    }

    // ============================================================
    // Create Tests - Rates Validation
    // ============================================================

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Create_ShouldReturnError_WhenCustomerRateIsInvalid(decimal invalidRate)
    {
        var result = TbDeliveryPricingRule.Create(
            Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Standard, null, invalidRate, 8m, null, null, null, null);

        Assert.False(result.IsSuccess);
        Assert.Equal(DeliveryRuleErrors.CustomerRatePerKmInvalid.Code, result.TopError.Code);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Create_ShouldReturnError_WhenDriverRateIsInvalid(decimal invalidRate)
    {
        var result = TbDeliveryPricingRule.Create(
            Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Standard, null, 10m, invalidRate, null, null, null, null);

        Assert.False(result.IsSuccess);
        Assert.Equal(DeliveryRuleErrors.DriverRatePerKmInvalid.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenDriverRateExceedsCustomerRate()
    {
        var result = TbDeliveryPricingRule.Create(
            Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Standard, null, 10m, 15m, null, null, null, null);

        Assert.False(result.IsSuccess);
        Assert.Equal(DeliveryRuleErrors.DriverRatePerKmExceedsCustomer.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenMaxDistanceIsInvalid()
    {
        var result = TbDeliveryPricingRule.Create(
            Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Standard, null, 10m, 8m, null, 0m, null, null);

        Assert.False(result.IsSuccess);
        Assert.Equal(DeliveryRuleErrors.MaxDistanceKmInvalid.Code, result.TopError.Code);
    }

    // ============================================================
    // Create Tests - Schedule Validation
    // ============================================================

    [Fact]
    public void Create_ShouldReturnError_WhenOnlyOneTimeIsSet()
    {
        var result = TbDeliveryPricingRule.Create(
            Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Standard, null, 10m, 8m, null, null,
            startHour: new TimeOnly(8, 0), endHour: null);

        Assert.False(result.IsSuccess);
        Assert.Equal(DeliveryRuleErrors.StartAndEndHourMustBothBeSet.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnError_WhenEndHourIsBeforeStartHour()
    {
        var result = TbDeliveryPricingRule.Create(
            Guid.NewGuid(), VehicleType.Motorcycle, ServiceTier.Standard, null, 10m, 8m, null, null,
            startHour: new TimeOnly(20, 0), endHour: new TimeOnly(8, 0));

        Assert.False(result.IsSuccess);
        Assert.Equal(DeliveryRuleErrors.EndHourMustBeAfterStartHour.Code, result.TopError.Code);
    }

    [Fact]
    public void Create_ShouldReturnSuccess_WhenDataIsValid()
    {
        var result = TbDeliveryPricingRule.Create(
            Guid.NewGuid(), VehicleType.Car, ServiceTier.Premium, CarType.Private,
            15m, 12m, 10m, 100m, new TimeOnly(9, 0), new TimeOnly(17, 0));

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.IsActive); // Should be active by default
    }

    // ============================================================
    // Update Tests
    // ============================================================

    [Fact]
    public void UpdateRates_ShouldReturnSuccess_AndUpdateProperties()
    {
        var rule = CreateValidRule();

        var result = rule.UpdateRates(20m, 15m, 10m);

        Assert.True(result.IsSuccess);
        Assert.Equal(20m, rule.CustomerRatePerKm);
        Assert.Equal(15m, rule.DriverRatePerKm);
        Assert.Equal(10m, rule.DriverRatePerOrder);
    }

    [Fact]
    public void UpdateRates_ShouldReturnError_WhenDriverRateExceedsCustomer()
    {
        var rule = CreateValidRule();

        var result = rule.UpdateRates(10m, 15m, 5m);

        Assert.False(result.IsSuccess);
        Assert.Equal(DeliveryRuleErrors.DriverRatePerKmExceedsCustomer.Code, result.TopError.Code);
    }

    [Fact]
    public void UpdateMaxDistance_ShouldReturnSuccess_AndUpdateProperty()
    {
        var rule = CreateValidRule();

        var result = rule.UpdateMaxDistance(100m);

        Assert.True(result.IsSuccess);
        Assert.Equal(100m, rule.MaxDistanceKm);
    }

    [Fact]
    public void UpdateSchedule_ShouldReturnSuccess_AndUpdateProperties()
    {
        var rule = CreateValidRule();
        var newStart = new TimeOnly(10, 0);
        var newEnd = new TimeOnly(22, 0);

        var result = rule.UpdateSchedule(newStart, newEnd);

        Assert.True(result.IsSuccess);
        Assert.Equal(newStart, rule.StartHour);
        Assert.Equal(newEnd, rule.EndHour);
    }

    // ============================================================
    // State Tests
    // ============================================================

    [Fact]
    public void Activate_ShouldReturnSuccess_AndSetIsActiveToTrue()
    {
        var rule = CreateValidRule();
        rule.Deactivate(); // Set to false first
        Assert.False(rule.IsActive);

        var result = rule.Activate();

        Assert.True(result.IsSuccess);
        Assert.True(rule.IsActive);
    }

    [Fact]
    public void Deactivate_ShouldReturnSuccess_AndSetIsActiveToFalse()
    {
        var rule = CreateValidRule();

        var result = rule.Deactivate();

        Assert.True(result.IsSuccess);
        Assert.False(rule.IsActive);
    }
}
