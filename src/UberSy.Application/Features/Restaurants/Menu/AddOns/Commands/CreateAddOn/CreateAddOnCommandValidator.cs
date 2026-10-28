using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.CreateAddOn;

public sealed class CreateAddOnCommandValidator : AbstractValidator<CreateAddOnCommand>
{
    public CreateAddOnCommandValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(100);
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
