using MediatR;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetRestaurantStats;

public sealed record GetRestaurantStatsQuery(Guid RestaurantId, DateOnly? From, DateOnly? To)
    : IRequest<Result<RestaurantStatsDto>>;
