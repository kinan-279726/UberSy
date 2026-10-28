using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Queries.GetAllRestaurant;

public sealed class GetAllRestaurantQueryValidator : AbstractValidator<GetAllRestaurantQuery>
{
    public GetAllRestaurantQueryValidator()
    {
        RuleFor(x => x.Search).MaximumLength(100);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}