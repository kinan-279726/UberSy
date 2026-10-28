using FluentValidation;
using UberSy.Application.Features.Restaurants.Menu.Items.requests.CreateItem;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Commands.CreateItem;

public sealed class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemCommandValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(100);
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DescriptionAr).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.DescriptionEn).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Calories).GreaterThanOrEqualTo(0).When(x => x.Calories.HasValue);
    }
}