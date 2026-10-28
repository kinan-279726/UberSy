using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Commands.CloseRestaurant;

public sealed record CloseRestaurantCommand(Guid RestaurantId) : IRequest<Result<Updated>>;
