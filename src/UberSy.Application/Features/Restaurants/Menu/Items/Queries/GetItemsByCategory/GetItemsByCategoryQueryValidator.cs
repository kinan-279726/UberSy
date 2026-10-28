using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Queries.GetItemsByCategory;

public sealed class GetItemsByCategoryQueryValidator : AbstractValidator<GetItemsByCategoryQuery>
{
    public GetItemsByCategoryQueryValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
    }
}