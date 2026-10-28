using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Constants;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Commands.UpdateAddOn;

public sealed class UpdateAddOnCommandHandler(IApplicationDbContext context ,IOwnershipValidator ownership)
    : IRequestHandler<UpdateAddOnCommand, Result<Updated>>
{
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Updated>> Handle(UpdateAddOnCommand request, CancellationToken cancellationToken)
    {
        var addOn = await _context.TbAddOns.FirstOrDefaultAsync(a => a.Id == request.AddOnId, cancellationToken);

        if (addOn is null)
        {
            return ApplicationErrors.ItemNotFound;
        }

        var isOwner = await _ownership.EnsureIsOwnerByRestaurantIdAsync(addOn.RestaurantId, cancellationToken);
        if (isOwner.IsError)
        {
            return isOwner.Errors;
        }
        
        var updateResult = addOn.Update(request.NameAr, request.NameEn , request.Price);

        if (updateResult.IsError)
        {
            return updateResult.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}