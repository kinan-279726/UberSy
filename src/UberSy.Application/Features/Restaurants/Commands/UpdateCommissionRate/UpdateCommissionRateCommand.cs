using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Commands.UpdateCommissionRate;

public sealed record UpdateCommissionRateCommand(Guid RestaurantId, decimal CommissionRate) : IRequest<Result<Updated>>;
