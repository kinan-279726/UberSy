using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Commands.UpdateTaxRate;

public sealed record UpdateTaxRateCommand(Guid RestaurantId, decimal TaxRate) : IRequest<Result<Updated>>;
