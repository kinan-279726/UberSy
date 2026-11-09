using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Commands.UpdateRestaurantLocation;

public sealed class UpdateRestaurantLocationCommandValidator : AbstractValidator<UpdateRestaurantLocationCommand>
{
    public UpdateRestaurantLocationCommandValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180);
    }
}
