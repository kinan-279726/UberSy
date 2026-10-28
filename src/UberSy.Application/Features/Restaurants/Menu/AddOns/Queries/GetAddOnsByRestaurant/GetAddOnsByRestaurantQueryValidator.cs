using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Queries.GetAddOnsByRestaurant;

public sealed class GetAddOnsByRestaurantQueryValidator : AbstractValidator<GetAddOnsByRestaurantQuery>
{
    public GetAddOnsByRestaurantQueryValidator() => RuleFor(x => x.RestaurantId).NotEmpty();
}