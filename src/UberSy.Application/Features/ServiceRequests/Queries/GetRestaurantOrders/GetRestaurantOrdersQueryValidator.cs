using FluentValidation;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetRestaurantOrders;

public sealed class GetRestaurantOrdersQueryValidator : AbstractValidator<GetRestaurantOrdersQuery>
{
    public GetRestaurantOrdersQueryValidator()
    {
        RuleFor(x => x.RestaurantId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}