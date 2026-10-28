using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.DeliveryPricingRules.Dtos;
using UberSy.Application.Features.DeliveryPricingRules.Mappers;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.DeliveryPricingRules.Queries.GetPricingRuleById;

public sealed class GetPricingRuleByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetPricingRuleByIdQuery, Result<DeliveryPricingRuleDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<DeliveryPricingRuleDto>> Handle(
        GetPricingRuleByIdQuery request, CancellationToken cancellationToken)
    {
        var rule = await _context.TbDeliveryPricingRules
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == request.RuleId && !r.IsDeleted, cancellationToken);

        return rule is null
            ? ApplicationErrors.DeliveryPricingRuleNotFound
            : rule.ToDto();
    }
}