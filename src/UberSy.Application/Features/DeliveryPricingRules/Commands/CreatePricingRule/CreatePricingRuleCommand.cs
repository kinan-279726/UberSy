using MediatR;
using UberSy.Application.Features.DeliveryPricingRules.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.DeliveryRule.Enums;

namespace UberSy.Application.Features.DeliveryPricingRules.Commands.CreatePricingRule;

public sealed record CreatePricingRuleCommand(
    VehicleType VehicleType, ServiceTier ServiceTier, CarType? CarType,
    decimal CustomerRatePerKm, decimal DriverRatePerKm, decimal? DriverRatePerOrder,
    decimal? MaxDistanceKm, TimeOnly? StartHour, TimeOnly? EndHour)
    : IRequest<Result<DeliveryPricingRuleDto>>;
