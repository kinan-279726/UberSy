using FluentValidation;
using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Offers.Commands.RejectOffer;

public sealed record RejectOfferCommand(Guid OfferId) : IRequest<Result<Updated>>;

public sealed class RejectOfferCommandValidator : AbstractValidator<RejectOfferCommand>
{
    public RejectOfferCommandValidator()
    {
        RuleFor(x => x.OfferId).NotEmpty();
    }
}
