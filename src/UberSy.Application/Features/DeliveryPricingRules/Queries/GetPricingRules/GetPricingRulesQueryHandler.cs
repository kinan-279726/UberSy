using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.DeliveryPricingRules.Dtos;
using UberSy.Application.Features.DeliveryPricingRules.Mappers;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.DeliveryPricingRules.Queries.GetPricingRules;

public sealed class GetPricingRulesQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetPricingRulesQuery, Result<List<DeliveryPricingRuleDto>>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<List<DeliveryPricingRuleDto>>> Handle(
        GetPricingRulesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TbDeliveryPricingRules.AsNoTracking().Where(r => !r.IsDeleted);

        if (request.VehicleType.HasValue)
            query = query.Where(r => r.VehicleType == request.VehicleType.Value);

        if (request.IsActive.HasValue)
            query = query.Where(r => r.IsActive == request.IsActive.Value);

        var rules = await query
            .OrderBy(r => r.VehicleType).ThenBy(r => r.ServiceTier)
            .ThenBy(r => r.CarType).ThenBy(r => r.StartHour)
            .ToListAsync(cancellationToken);

        return rules.Select(r => r.ToDto()).ToList();
    }
}