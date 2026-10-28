using FluentValidation;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Queries.GetItemById;

public sealed class GetItemByIdQueryValidator : AbstractValidator<GetItemByIdQuery>
{
    public GetItemByIdQueryValidator()
    {
        RuleFor(x => x.ItemId).NotEmpty();
    }
}