using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Commands.UpdateOperatingHours;

public sealed class UpdateOperatingHoursCommandValidator : AbstractValidator<UpdateOperatingHoursCommand>
{
    public UpdateOperatingHoursCommandValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.ClosingTime).GreaterThan(x => x.OpeningTime);
    }
}
