using MediatR;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Drivers.Queries.MyPendingOffers;

public sealed record GetMyPendingOffersQuery : IRequest<Result<List<DriverOfferSummaryDto>>>;
