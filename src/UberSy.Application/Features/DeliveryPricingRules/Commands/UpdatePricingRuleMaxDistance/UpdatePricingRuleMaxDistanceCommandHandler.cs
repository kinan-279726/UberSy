using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Application.Features.DeliveryPricingRules.Commands.UpdatePricingRuleMaxDistance;

public sealed class UpdatePricingRuleMaxDistanceCommandHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<UpdatePricingRuleMaxDistanceCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUser _user = user;

    public async Task<Result<Updated>> Handle(
        UpdatePricingRuleMaxDistanceCommand request, CancellationToken cancellationToken)
    {
        if (!_user.IsInRole(Roles.Admin)) return ApplicationErrors.AccessDenied;

        var rule = await _context.TbDeliveryPricingRules
            .FirstOrDefaultAsync(r => r.Id == request.RuleId, cancellationToken);

        if (rule is null)
            return ApplicationErrors.DeliveryPricingRuleNotFound;

        var result = rule.UpdateMaxDistance(request.MaxDistanceKm);
        if (result.IsError)
            return result.Errors;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}