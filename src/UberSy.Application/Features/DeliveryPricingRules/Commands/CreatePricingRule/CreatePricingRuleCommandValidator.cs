using FluentValidation;

namespace UberSy.Application.Features.DeliveryPricingRules.Commands.CreatePricingRule;

public sealed class CreatePricingRuleCommandValidator : AbstractValidator<CreatePricingRuleCommand>
{
    public CreatePricingRuleCommandValidator()
    {
        RuleFor(x => x.VehicleType).IsInEnum();
        RuleFor(x => x.ServiceTier).IsInEnum();
        RuleFor(x => x.CarType).IsInEnum().When(x => x.CarType.HasValue);
        RuleFor(x => x.CustomerRatePerKm).GreaterThan(0);
        RuleFor(x => x.DriverRatePerKm).GreaterThan(0).LessThanOrEqualTo(x => x.CustomerRatePerKm);
        RuleFor(x => x.DriverRatePerOrder).GreaterThan(0).When(x => x.DriverRatePerOrder.HasValue);
        RuleFor(x => x.MaxDistanceKm).GreaterThan(0).When(x => x.MaxDistanceKm.HasValue);
        RuleFor(x => x).Must(x => x.StartHour.HasValue == x.EndHour.HasValue)
            .WithMessage("StartHour and EndHour must both be set or both be empty.");
        RuleFor(x => x.EndHour).GreaterThan(x => x.StartHour)
            .When(x => x.StartHour.HasValue && x.EndHour.HasValue);
    }
}
