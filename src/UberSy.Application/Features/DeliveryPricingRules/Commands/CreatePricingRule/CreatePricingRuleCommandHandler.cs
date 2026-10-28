using MediatR;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.DeliveryPricingRules.Dtos;
using UberSy.Application.Features.DeliveryPricingRules.Mappers;
using UberSy.Domain.Common.Results;
using UberSy.Domain.DeliveryRule;
using UberSy.Domain.Identity;

namespace UberSy.Application.Features.DeliveryPricingRules.Commands.CreatePricingRule;

public sealed class CreatePricingRuleCommandHandler(
    IApplicationDbContext context,
    IUser user,
    ILogger<CreatePricingRuleCommandHandler> logger)
    : IRequestHandler<CreatePricingRuleCommand, Result<DeliveryPricingRuleDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUser _user = user;
    private readonly ILogger<CreatePricingRuleCommandHandler> _logger = logger;

    public async Task<Result<DeliveryPricingRuleDto>> Handle(
        CreatePricingRuleCommand request, CancellationToken cancellationToken)
    {
        if (!_user.IsInRole(Roles.Admin)) return ApplicationErrors.AccessDenied;

        var createResult = TbDeliveryPricingRule.Create(
            Guid.NewGuid(), request.VehicleType, request.ServiceTier, request.CarType,
            request.CustomerRatePerKm, request.DriverRatePerKm, request.DriverRatePerOrder,
            request.MaxDistanceKm, request.StartHour, request.EndHour);

        if (createResult.IsError)
            return createResult.Errors;

        var rule = createResult.Value;

        if (await PricingRuleScheduleChecker.HasOverlapAsync(
                _context, rule.VehicleType, rule.ServiceTier, rule.CarType,
                rule.StartHour, rule.EndHour, null, cancellationToken))
        {
            return ApplicationErrors.PricingRuleScheduleOverlap;
        }

        _context.TbDeliveryPricingRules.Add(rule);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Pricing rule '{RuleId}' created.", rule.Id);

        return rule.ToDto();
    }
}
