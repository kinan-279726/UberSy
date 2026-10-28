using FluentValidation;

namespace UberSy.Application.Features.Drivers.Queries.MyWalletTransactions;

public sealed class GetMyWalletTransactionsQueryValidator : AbstractValidator<GetMyWalletTransactionsQuery>
{
    public GetMyWalletTransactionsQueryValidator()
    {
        RuleFor(x => x.Type).IsInEnum().When(x => x.Type.HasValue);
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 50);
    }
}