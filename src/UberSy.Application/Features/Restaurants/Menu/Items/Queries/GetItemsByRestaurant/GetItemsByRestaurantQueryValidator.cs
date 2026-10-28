using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Queries.GetItemsByRestaurant;

public sealed class GetItemsByRestaurantQueryValidator : AbstractValidator<GetItemsByRestaurantQuery>
{
    public GetItemsByRestaurantQueryValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
    }
}