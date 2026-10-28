using UberSy.Application.Features.DeliveryPricingRules.Dtos;
using UberSy.Domain.DeliveryRule;
using UberSy.Domain.DeliveryRule.Enums;

namespace UberSy.Application.Features.DeliveryPricingRules.Mappers;

public static class DeliveryPricingRuleMapper
{
    public static DeliveryPricingRuleDto ToDto(this TbDeliveryPricingRule r) => new(
        r.Id, (VehicleType)r.VehicleType, (ServiceTier)r.ServiceTier,
        r.CarType.HasValue ? (CarType)r.CarType.Value : null,
        r.CustomerRatePerKm, r.DriverRatePerKm, r.DriverRatePerOrder,
        r.MaxDistanceKm, r.StartHour, r.EndHour, r.IsActive);

    public static List<DeliveryPricingRuleDto> ToDtos(this List<TbDeliveryPricingRule> entities)
    {
        return [..entities.Select(e => e.ToDto())];
    }
}