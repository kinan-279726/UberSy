using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Commands.UpdateCategoryProfile;

public sealed class UpdateCategoryProfileCommandValidator : AbstractValidator<UpdateCategoryProfileCommand>
{
    public UpdateCategoryProfileCommandValidator()
    {
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(100);
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(100);
    }
}