using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Commands.OpenRestaurant;
public sealed record OpenRestaurantCommand(Guid RestaurantId) : IRequest<Result<Updated>>;

