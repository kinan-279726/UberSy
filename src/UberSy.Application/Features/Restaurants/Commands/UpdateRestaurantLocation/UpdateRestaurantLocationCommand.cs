using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Commands.UpdateRestaurantLocation;

public sealed record UpdateRestaurantLocationCommand(Guid RestaurantId, double Latitude, double Longitude) : IRequest<Result<Updated>>;
