using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Commands.UpdateRestaurantProfile;

public sealed class UpdateRestaurantProfileCommandValidator : AbstractValidator<UpdateRestaurantProfileCommand>
{
    public UpdateRestaurantProfileCommandValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(100);
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Phone).NotEmpty().MaximumLength(20);
    }
}
