using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Commands.ApproveRestaurant;
public sealed record ApproveRestaurantCommand(Guid RestaurantId) : IRequest<Result<Updated>>;

