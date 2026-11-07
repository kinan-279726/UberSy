using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Offers.Commands.RejectOffer;

public sealed record RejectOfferCommand(Guid OfferId) : IRequest<Result<Updated>>;
