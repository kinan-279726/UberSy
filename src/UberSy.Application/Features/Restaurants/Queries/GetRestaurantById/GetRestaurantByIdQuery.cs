using MediatR;
using UberSy.Application.Features.Restaurants.Dtos;
using UberSy.Domain.Common.Results;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace UberSy.Application.Features.Restaurants.Queries.GetRestaurantById;

public sealed record GetRestaurantByIdQuery(Guid RestaurantId) : IRequest<Result<RestaurantDto>>;
