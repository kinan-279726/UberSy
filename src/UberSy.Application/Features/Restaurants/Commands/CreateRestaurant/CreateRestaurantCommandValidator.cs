using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Commands.CreateRestaurant;

public sealed class CreateRestaurantCommandValidator : AbstractValidator<CreateRestaurantCommand>
{
    public CreateRestaurantCommandValidator()
    {
        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Arabic Name Is Required.")
            .MaximumLength(100);

        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("English Name Is Required.")
            .MaximumLength(100);

        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone Is Required.")
            .MaximumLength(20);

        RuleFor(x => x.CommissionRate)
            .InclusiveBetween(0, 100).WithMessage("Commission Rate Must Be Between 0 And 100.");

        RuleFor(x => x.ClosingTime)
            .GreaterThan(x => x.OpeningTime).WithMessage("Closing Time Must Be After Opening Time.");

        RuleFor(x => x.MaxOrderAmount)
            .GreaterThan(x => x.MinOrderAmount).WithMessage("Max Order Amount Must Be Greater Than Min.");
    }
}