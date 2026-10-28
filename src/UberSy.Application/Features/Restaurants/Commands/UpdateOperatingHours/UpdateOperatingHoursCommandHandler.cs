using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Commands.UpdateOperatingHours;

public sealed class UpdateOperatingHoursCommandHandler(IApplicationDbContext context , IOwnershipValidator ownership) : IRequestHandler<UpdateOperatingHoursCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<Updated>> Handle(UpdateOperatingHoursCommand request, CancellationToken cancellationToken)
    {
        var restaurant = await _context.TbRestaurants.FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);
        if (restaurant is null) return ApplicationErrors.RestaurantNotFound;

        var isOwner = await _ownership.EnsureIsOwnerByOwnerIdAsync(restaurant.OwnerId, cancellationToken);
        if (isOwner.IsError)
        {
            return isOwner.Errors;
        }

        var result = restaurant.UpdateOperatingHours(request.OpeningTime, request.ClosingTime);
        if (result.IsError) return result.Errors;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}
