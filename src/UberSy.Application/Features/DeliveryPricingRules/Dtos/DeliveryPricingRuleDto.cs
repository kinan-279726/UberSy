using UberSy.Domain.DeliveryRule.Enums;

namespace UberSy.Application.Features.DeliveryPricingRules.Dtos;

public sealed record DeliveryPricingRuleDto(
Guid Id, VehicleType VehicleType, ServiceTier ServiceTier, CarType? CarType,
decimal CustomerRatePerKm, decimal DriverRatePerKm, decimal? DriverRatePerOrder,
decimal? MaxDistanceKm, TimeOnly? StartHour, TimeOnly? EndHour, bool IsActive);
