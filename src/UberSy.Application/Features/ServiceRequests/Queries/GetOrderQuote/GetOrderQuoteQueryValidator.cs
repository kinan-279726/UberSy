using FluentValidation;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetOrderQuote;

public sealed class GetOrderQuoteQueryValidator : AbstractValidator<GetOrderQuoteQuery>
{
    public GetOrderQuoteQueryValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.AddressId).NotEmpty();
        RuleFor(x => x.Items).NotEmpty().Must(i => i.Count <= 50)
            .WithMessage("An order cannot contain more than 50 lines.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ItemId).NotEmpty();
            item.RuleFor(i => i.Quantity).InclusiveBetween(1, 99);
            item.RuleFor(i => i.AddOnIds).NotNull();
        });
    }
}
