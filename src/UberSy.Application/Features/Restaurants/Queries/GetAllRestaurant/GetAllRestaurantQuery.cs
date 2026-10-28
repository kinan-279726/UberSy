using MediatR;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Restaurants.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Queries.GetAllRestaurant;

public sealed record GetAllRestaurantQuery(
    string? Search,
    bool? IsOpenNow,
    int PageNumber = 1,
    int PageSize = 20) : IRequest<Result<PagedList<RestaurantListItemDto>>>;