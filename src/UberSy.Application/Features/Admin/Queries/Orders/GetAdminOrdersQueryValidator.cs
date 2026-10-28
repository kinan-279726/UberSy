using FluentValidation;

namespace UberSy.Application.Features.Admin.Queries.Orders;

public sealed class GetAdminOrdersQueryValidator : AbstractValidator<GetAdminOrdersQuery>
{
    public GetAdminOrdersQueryValidator()
    {
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x).Must(x => !x.From.HasValue || !x.To.HasValue || x.From <= x.To)
            .WithMessage("From must be before or equal to To.");
    }
}
