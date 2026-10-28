using FluentValidation;

namespace UberSy.Application.Features.ServiceRequests.Commands.PlaceFoodOrder;

public sealed class PlaceFoodOrderCommandValidator : AbstractValidator<PlaceFoodOrderCommand>
{
    public PlaceFoodOrderCommandValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.AddressId).NotEmpty();

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order Must Contain At Least One Item.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ItemId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}