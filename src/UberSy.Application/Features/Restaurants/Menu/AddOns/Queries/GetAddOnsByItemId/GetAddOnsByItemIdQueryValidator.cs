using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Queries.GetAddOnsByItemId;

public sealed class GetAddOnsByItemIdQueryValidator : AbstractValidator<GetAddOnsByItemIdQuery>
{
    public GetAddOnsByItemIdQueryValidator() => RuleFor(x => x.ItemId).NotEmpty();
}