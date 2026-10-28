using FluentValidation;

namespace UberSy.Application.Features.Drivers.Queries.FindDriverForTopUp;

public sealed class FindDriverForTopUpQueryValidator : AbstractValidator<FindDriverForTopUpQuery>
{
    public FindDriverForTopUpQueryValidator()
    {
        RuleFor(x => x.Search).NotEmpty().MaximumLength(30);
    }
}
