using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.DeliveryPricingRules.Commands.DeletePricingRule;

public sealed record DeletePricingRuleCommand(Guid RuleId) : IRequest<Result<Deleted>>;
