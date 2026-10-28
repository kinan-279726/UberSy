using FluentValidation;

namespace UberSy.Application.Features.Admin.Queries.GetDrivers;

public sealed class GetDriversQueryValidator : AbstractValidator<GetDriversQuery>
{
    public GetDriversQueryValidator()
    {
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
        RuleFor(x => x.Search).MaximumLength(50);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}