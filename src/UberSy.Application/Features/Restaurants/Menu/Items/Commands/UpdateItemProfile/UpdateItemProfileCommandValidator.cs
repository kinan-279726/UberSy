using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Commands.UpdateItemProfile;

public sealed class UpdateItemProfileCommandValidator : AbstractValidator<UpdateItemProfileCommand>
{
    public UpdateItemProfileCommandValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.NameAr).NotEmpty().MaximumLength(100);
        RuleFor(x => x.NameEn).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DescriptionAr).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.DescriptionEn).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.ImageUrl).NotEmpty().MaximumLength(1000);
    }
}
