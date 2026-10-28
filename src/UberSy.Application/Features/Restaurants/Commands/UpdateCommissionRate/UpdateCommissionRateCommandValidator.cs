using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Commands.UpdateCommissionRate;

public sealed class UpdateCommissionRateCommandValidator : AbstractValidator<UpdateCommissionRateCommand>
{
    public UpdateCommissionRateCommandValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.CommissionRate).InclusiveBetween(0, 100);
    }
}