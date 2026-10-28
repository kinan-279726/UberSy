using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Commands.UpdateItemPrice;

public sealed class UpdateItemPriceCommandValidator : AbstractValidator<UpdateItemPriceCommand>
{
    public UpdateItemPriceCommandValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
    }
}