using FluentValidation;

namespace UberSy.Application.Features.ServiceRequests.Offers.Commands.AcceptOffer;

public sealed class AcceptOfferCommandValidator : AbstractValidator<AcceptOfferCommand>
{
    public AcceptOfferCommandValidator()
    {
        RuleFor(o => o.OfferId).NotEmpty();
    }
}
