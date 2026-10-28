using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Commands.RejectRestaurant;

public sealed record RejectRestaurantCommand(Guid RestaurantId) : IRequest<Result<Updated>>;
