using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Commands.UpdateTaxRate;

// Validator: TaxRate بين 0 و100
// Handler: restaurant.UpdateTaxRate(command.TaxRate)
public sealed class UpdateTaxRateCommandValidator : AbstractValidator<UpdateTaxRateCommand>
{
    public UpdateTaxRateCommandValidator()
    {
        RuleFor(x => x.TaxRate).NotEmpty().InclusiveBetween(0, 100);
        RuleFor(x => x.RestaurantId).NotEmpty();
    }
}