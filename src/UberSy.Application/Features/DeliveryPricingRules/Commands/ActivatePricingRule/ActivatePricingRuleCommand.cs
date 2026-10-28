using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.DeliveryPricingRules.Commands.ActivatePricingRule;

public sealed record ActivatePricingRuleCommand(Guid RuleId) : IRequest<Result<Updated>>;
