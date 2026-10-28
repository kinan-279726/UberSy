using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Commands.UpdateOrderLimits;

public sealed class UpdateOrderLimitsCommandValidator : AbstractValidator<UpdateOrderLimitsCommand>
{
    public UpdateOrderLimitsCommandValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.MinOrderAmount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MaxOrderAmount).GreaterThan(x => x.MinOrderAmount);
    }
}
