using MediatR;
using UberSy.Application.Features.Restaurants.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Queries.GetRestaurantsByOwner;

public sealed record GetRestaurantsByOwnerQuery : IRequest<Result<List<RestaurantDto>>>;
