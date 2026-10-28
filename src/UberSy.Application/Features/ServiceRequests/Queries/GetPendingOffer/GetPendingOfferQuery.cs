using MediatR;
using UberSy.Application.Features.ServiceRequests.Offers.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetPendingOffer;

public sealed record GetPendingOfferQuery(Guid OfferId) : IRequest<Result<PendingOfferDto>>;
