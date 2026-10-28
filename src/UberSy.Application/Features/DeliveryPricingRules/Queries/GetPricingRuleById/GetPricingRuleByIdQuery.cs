using MediatR;
using UberSy.Application.Features.DeliveryPricingRules.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.DeliveryPricingRules.Queries.GetPricingRuleById;

public sealed record GetPricingRuleByIdQuery(Guid RuleId) : IRequest<Result<DeliveryPricingRuleDto>>;
