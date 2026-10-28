using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Commands.ActivateItem;

public sealed class ActivateItemCommandHandler(IApplicationDbContext context, IOwnershipValidator ownership) : IRequestHandler<ActivateItemCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<Updated>> Handle(ActivateItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.TbItems.FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);
        if (item is null) return ApplicationErrors.ItemNotFound;

        var isOwner = await _ownership.EnsureIsOwnerByRestaurantIdAsync(item.RestaurantId, cancellationToken);
        if (isOwner.IsError)
            return isOwner.Errors;

        var updateResult = item.Activate();

        if (updateResult.IsError) return updateResult.Errors;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;

    }
}