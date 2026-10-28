using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Constants;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.DeactivateAddOn;

public sealed class DeactivateAddOnCommandHandler(IApplicationDbContext context , IOwnershipValidator ownership)
    : IRequestHandler<DeactivateAddOnCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<Updated>> Handle(DeactivateAddOnCommand requst, CancellationToken cancellationToken)
    {
        var addOn = await _context.TbAddOns.FirstOrDefaultAsync(a => a.Id == requst.AddOnId, cancellationToken);

        if (addOn is null) return ApplicationErrors.ItemNotFound;

        var isOwner = await _ownership.EnsureIsOwnerByRestaurantIdAsync(addOn.RestaurantId, cancellationToken);
        if (isOwner.IsError)
            return isOwner.Errors;

        var result = addOn.Deactivate();

        if (result.IsError) return result.Errors;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}