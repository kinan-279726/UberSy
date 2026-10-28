using MediatR;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Restaurants.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Queries.GetPendingRestaurants;
public sealed record GetPendingRestaurantsQuery(int PageNumber = 1, int PageSize = 20)
    : IRequest<Result<PagedList<RestaurantDto>>>;
