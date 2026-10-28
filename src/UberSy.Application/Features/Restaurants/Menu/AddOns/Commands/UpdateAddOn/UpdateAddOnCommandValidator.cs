using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.UpdateAddOn;

public sealed class UpdateAddOnCommandValidator : AbstractValidator<UpdateAddOnCommand>
{
    public UpdateAddOnCommandValidator()
    {
        RuleFor(x => x.AddOnId).NotEmpty();
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(100);
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

