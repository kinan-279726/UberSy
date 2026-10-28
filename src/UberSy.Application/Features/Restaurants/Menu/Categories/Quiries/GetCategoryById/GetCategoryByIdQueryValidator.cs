using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Quiries.GetCategoryById;

public sealed class GetCategoryByIdQueryValidator : AbstractValidator<GetCategoryByIdQuery>
{
    public GetCategoryByIdQueryValidator() => RuleFor(x => x.CategoryId).NotEmpty();
}