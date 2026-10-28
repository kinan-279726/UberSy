using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Application.Features.DeliveryPricingRules.Commands.ActivatePricingRule;

public sealed class ActivatePricingRuleCommandHandler(IApplicationDbContext context, IUser user)
    : IRequestHandler<ActivatePricingRuleCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUser _user = user;

    public async Task<Result<Updated>> Handle(
        ActivatePricingRuleCommand request, CancellationToken cancellationToken)
    {
        if (!_user.IsInRole(Roles.Admin)) return ApplicationErrors.AccessDenied;

        var rule = await _context.TbDeliveryPricingRules
            .FirstOrDefaultAsync(r => r.Id == request.RuleId, cancellationToken);

        if (rule is null)
            return ApplicationErrors.DeliveryPricingRuleNotFound;

        if (rule.IsActive)
            return Result.Updated;

        if (await PricingRuleScheduleChecker.HasOverlapAsync(
                _context, rule.VehicleType, rule.ServiceTier, rule.CarType,
                rule.StartHour, rule.EndHour, rule.Id, cancellationToken))
        {
            return ApplicationErrors.PricingRuleScheduleOverlap;
        }

        rule.Activate();
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}