using UberSy.Application.Features.DeliveryPricingRules.Commands.ActivatePricingRule;
using UberSy.Application.Features.DeliveryPricingRules.Commands.CreatePricingRule;
using UberSy.Application.Features.DeliveryPricingRules.Commands.DeactivatePricingRule;
using UberSy.Application.Features.DeliveryPricingRules.Commands.DeletePricingRule;
using UberSy.Application.Features.DeliveryPricingRules.Commands.UpdatePricingRuleMaxDistance;
using UberSy.Application.Features.DeliveryPricingRules.Commands.UpdateRates;
using UberSy.Application.Features.DeliveryPricingRules.Commands.UpdateSchedule;
using UberSy.Domain.DeliveryRule.Enums;

namespace UberSy.Test.Common.DeliveryPricingRule;

public static class PricingRuleCommandFactory
{
    // 1. Create Pricing Rule
    public static CreatePricingRuleCommand CreateCreatePricingRuleCommand(
        VehicleType? vehicleType = null,
        ServiceTier? serviceTier = null,
        CarType? carType = null,
        decimal? customerRatePerKm = null,
        decimal? driverRatePerKm = null,
        decimal? driverRatePerOrder = null,
        decimal? maxDistanceKm = null,
        TimeOnly? startHour = null,
        TimeOnly? endHour = null)
    {
        var vt = vehicleType ?? VehicleType.Motorcycle;

        var validCarType = vt == VehicleType.Car ? carType : null;

        return new CreatePricingRuleCommand(
            vt,
            serviceTier ?? ServiceTier.Standard,
            validCarType,
            customerRatePerKm ?? 5m,
            driverRatePerKm ?? 3m,
            driverRatePerOrder,
            maxDistanceKm,
            startHour,
            endHour);
    }

    // 2. Activate Pricing Rule
    public static ActivatePricingRuleCommand CreateActivatePricingRuleCommand(
        Guid? ruleId = null)
    {
        return new ActivatePricingRuleCommand(
            ruleId ?? Guid.NewGuid());
    }

    // 3. Deactivate Pricing Rule
    public static DeactivatePricingRuleCommand CreateDeactivatePricingRuleCommand(
        Guid? ruleId = null)
    {
        return new DeactivatePricingRuleCommand(
            ruleId ?? Guid.NewGuid());
    }

    // 4. Delete Pricing Rule
    public static DeletePricingRuleCommand CreateDeletePricingRuleCommand(
        Guid? ruleId = null)
    {
        return new DeletePricingRuleCommand(
            ruleId ?? Guid.NewGuid());
    }

    // 5. Update Max Distance
    public static UpdatePricingRuleMaxDistanceCommand CreateUpdatePricingRuleMaxDistanceCommand(
        Guid? ruleId = null,
        decimal? maxDistanceKm = null)
    {
        return new UpdatePricingRuleMaxDistanceCommand(
            ruleId ?? Guid.NewGuid(),
            maxDistanceKm);
    }

    // 6. Update Rates
    public static UpdatePricingRuleRatesCommand CreateUpdatePricingRuleRatesCommand(
        Guid? ruleId = null,
        decimal? customerRatePerKm = null,
        decimal? driverRatePerKm = null,
        decimal? driverRatePerOrder = null)
    {
        var custRate = customerRatePerKm ?? 5m;
        var driverRate = driverRatePerKm ?? 3m;

        return new UpdatePricingRuleRatesCommand(
            ruleId ?? Guid.NewGuid(),
            custRate,
            driverRate,
            driverRatePerOrder);
    }

    // 7. Update Schedule
    public static UpdatePricingRuleScheduleCommand CreateUpdatePricingRuleScheduleCommand(
        Guid? ruleId = null,
        TimeOnly? startHour = null,
        TimeOnly? endHour = null)
    {
        if (startHour is null && endHour is null)
        {
            startHour = new TimeOnly(8, 0);
            endHour = new TimeOnly(22, 0);
        }

        return new UpdatePricingRuleScheduleCommand(
            ruleId ?? Guid.NewGuid(),
            startHour,
            endHour);
    }
}
