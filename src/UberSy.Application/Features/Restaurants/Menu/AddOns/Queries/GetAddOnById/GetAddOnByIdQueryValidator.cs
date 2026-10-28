using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Queries.GetAddOnById;

public sealed class GetAddOnByIdQueryValidator : AbstractValidator<GetAddOnByIdQuery>
{
    public GetAddOnByIdQueryValidator() => RuleFor(x => x.AddOnId).NotEmpty();
}
