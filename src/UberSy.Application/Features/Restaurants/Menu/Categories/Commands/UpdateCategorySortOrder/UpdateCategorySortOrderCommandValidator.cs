using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Commands.UpdateCategorySortOrder;

public sealed class UpdateCategorySortOrderCommandValidator : AbstractValidator<UpdateCategorySortOrderCommand>
{
    public UpdateCategorySortOrderCommandValidator() => RuleFor(x => x.CategoryId).NotEmpty();
}