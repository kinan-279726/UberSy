using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Quiries.GetCategoriesByRestaurant;

public sealed class GetCategoriesByRestaurantQueryValidator : AbstractValidator<GetCategoriesByRestaurantQuery>
{
    public GetCategoriesByRestaurantQueryValidator() => RuleFor(x => x.RestaurantId).NotEmpty();
}