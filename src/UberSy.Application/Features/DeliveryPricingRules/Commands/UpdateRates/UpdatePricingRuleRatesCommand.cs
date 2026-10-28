using MediatR;
using UberSy.Application.Features.DeliveryPricingRules.Commands.UpdatePricingRuleMaxDistance;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.DeliveryPricingRules.Commands.UpdateRates;

public sealed record UpdatePricingRuleRatesCommand(
    Guid RuleId, decimal CustomerRatePerKm, decimal DriverRatePerKm, decimal? DriverRatePerOrder)
    : IRequest<Result<Updated>>;
