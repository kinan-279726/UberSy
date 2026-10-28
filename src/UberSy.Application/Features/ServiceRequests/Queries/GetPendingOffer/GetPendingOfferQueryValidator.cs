using FluentValidation;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetPendingOffer;

public sealed class GetPendingOfferQueryValidator : AbstractValidator<GetPendingOfferQuery>
{
    public GetPendingOfferQueryValidator() => RuleFor(x => x.OfferId).NotEmpty();
}
