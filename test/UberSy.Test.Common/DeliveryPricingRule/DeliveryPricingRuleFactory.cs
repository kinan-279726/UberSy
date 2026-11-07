using UberSy.Domain.Common.Results;
using UberSy.Domain.DeliveryRule;
using UberSy.Domain.DeliveryRule.Enums;

namespace UberSy.Test.Common.DeliveryPricingRule;

public static class DeliveryPricingRuleFactory
{
    public static Result<TbDeliveryPricingRule> CreateDeliveryPricingRule(
        Guid? id = null,
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
        return TbDeliveryPricingRule.Create(
            id ?? Guid.NewGuid(),
            vehicleType ?? VehicleType.Motorcycle,
            serviceTier ?? ServiceTier.Standard,
            carType,
            customerRatePerKm ?? 5m,
            driverRatePerKm ?? 3m,
            driverRatePerOrder,
            maxDistanceKm,
            startHour,
            endHour);
    }
}
