using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Application.Features.DeliveryPricingRules.Commands.UpdateSchedule;

public sealed class UpdatePricingRuleScheduleCommandHandler(IApplicationDbContext context , IUser user)
    : IRequestHandler<UpdatePricingRuleScheduleCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUser _user = user;

    public async Task<Result<Updated>> Handle(
        UpdatePricingRuleScheduleCommand request, CancellationToken cancellationToken)
    {
        if (!_user.IsInRole(Roles.Admin)) return ApplicationErrors.AccessDenied;

        var rule = await _context.TbDeliveryPricingRules
            .FirstOrDefaultAsync(r => r.Id == request.RuleId, cancellationToken);

        if (rule is null)
            return ApplicationErrors.DeliveryPricingRuleNotFound;

        if (rule.IsActive && await PricingRuleScheduleChecker.HasOverlapAsync(
                _context, rule.VehicleType, rule.ServiceTier, rule.CarType,
                request.StartHour, request.EndHour, rule.Id, cancellationToken))
        {
            return ApplicationErrors.PricingRuleScheduleOverlap;
        }

        var result = rule.UpdateSchedule(request.StartHour, request.EndHour);
        if (result.IsError)
            return result.Errors;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}