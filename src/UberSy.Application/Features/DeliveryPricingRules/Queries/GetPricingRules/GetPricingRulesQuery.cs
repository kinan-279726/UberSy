using MediatR;
using UberSy.Application.Features.DeliveryPricingRules.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.DeliveryRule.Enums;

namespace UberSy.Application.Features.DeliveryPricingRules.Queries.GetPricingRules;

public sealed record GetPricingRulesQuery(VehicleType? VehicleType, bool? IsActive)
    : IRequest<Result<List<DeliveryPricingRuleDto>>>;
