using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Commands.UpdateRestaurantImages;

public sealed record UpdateRestaurantImagesCommand(Guid RestaurantId, string? LogoUrl, string? CoverUrl) : IRequest<Result<Updated>>;
