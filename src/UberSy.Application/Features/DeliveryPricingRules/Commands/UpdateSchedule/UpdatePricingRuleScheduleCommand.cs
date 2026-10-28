using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.DeliveryPricingRules.Commands.UpdateSchedule;

public sealed record UpdatePricingRuleScheduleCommand(Guid RuleId, TimeOnly? StartHour, TimeOnly? EndHour)
    : IRequest<Result<Updated>>;
