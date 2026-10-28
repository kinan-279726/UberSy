using FluentValidation;

namespace UberSy.Application.Features.DeliveryPricingRules.Commands.UpdateSchedule;
public sealed class UpdatePricingRuleScheduleCommandValidator : AbstractValidator<UpdatePricingRuleScheduleCommand>
{
    public UpdatePricingRuleScheduleCommandValidator()
    {
        RuleFor(x => x.RuleId).NotEmpty();
        RuleFor(x => x).Must(x => x.StartHour.HasValue == x.EndHour.HasValue)
            .WithMessage("StartHour and EndHour must both be set or both be empty.");
        RuleFor(x => x.EndHour).GreaterThan(x => x.StartHour)
            .When(x => x.StartHour.HasValue && x.EndHour.HasValue);
    }
}
