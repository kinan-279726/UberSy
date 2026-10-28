using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Commands.UpdateOrderLimits;

public sealed record UpdateOrderLimitsCommand(Guid RestaurantId, decimal MinOrderAmount, decimal MaxOrderAmount) : IRequest<Result<Updated>>;
