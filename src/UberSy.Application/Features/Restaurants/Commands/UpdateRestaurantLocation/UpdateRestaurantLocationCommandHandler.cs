using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Constants;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Commands.UpdateRestaurantLocation;

public sealed class UpdateRestaurantLocationCommandHandler(IApplicationDbContext context ,IOwnershipValidator ownership)
    : IRequestHandler<UpdateRestaurantLocationCommand, Result<Updated>>
{
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Updated>> Handle(UpdateRestaurantLocationCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _context.TbRestaurants.FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);
        if (restaurant is null) return ApplicationErrors.RestaurantNotFound;

        var isOwner = await _ownership.EnsureIsOwnerByOwnerIdAsync(restaurant.OwnerId, cancellationToken);
        if (isOwner.IsError)
            return isOwner.Errors;

        var point = new Point(request.Longitude, request.Latitude) { SRID = 4326 };
        var result = restaurant.UpdateBoundary(point);
        if (result.IsError) return result.Errors;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
