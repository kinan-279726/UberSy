using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.DeliveryPricingRules.Commands.UpdatePricingRuleMaxDistance;

public sealed record UpdatePricingRuleMaxDistanceCommand(Guid RuleId, decimal? MaxDistanceKm)
    : IRequest<Result<Updated>>;
