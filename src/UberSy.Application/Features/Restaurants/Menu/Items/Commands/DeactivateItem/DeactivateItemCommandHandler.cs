using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Commands.DeactivateItem;

public sealed class DeactivateItemCommandHandler(IApplicationDbContext context, IOwnershipValidator ownership)
    : IRequestHandler<DeactivateItemCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<Updated>> Handle(DeactivateItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.TbItems.FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);
        if (item is null) return ApplicationErrors.ItemNotFound;

        var isOwner = await _ownership.EnsureIsOwnerByRestaurantIdAsync(item.RestaurantId, cancellationToken);
        if (isOwner.IsError)
            return isOwner.Errors;

        var updateResult = item.Deactivate();
        if (updateResult.IsError) return updateResult.Errors;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;

    }
}
