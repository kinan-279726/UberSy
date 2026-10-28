using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Offers.Commands.AcceptOffer;

public sealed record AcceptOfferCommand(Guid OfferId) : IRequest<Result<Updated>>;