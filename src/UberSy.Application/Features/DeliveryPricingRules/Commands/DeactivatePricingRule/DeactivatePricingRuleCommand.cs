using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.DeliveryPricingRules.Commands.DeactivatePricingRule;

public sealed record DeactivatePricingRuleCommand(Guid RuleId) : IRequest<Result<Updated>>;