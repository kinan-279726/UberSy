using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Commands.ActivateCategory;


public sealed class ActivateCategoryCommandHandler(IApplicationDbContext context, IOwnershipValidator ownership)
    : IRequestHandler<ActivateCategoryCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<Updated>> Handle(ActivateCategoryCommand request, CancellationToken cancellation)
    {
        var category = await _context.TbCategories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellation);
        if (category is null) return ApplicationErrors.CategoryNotFound;

        var isOwner = await _ownership.EnsureIsOwnerByRestaurantIdAsync(category.RestaurantId, cancellation);
        if (isOwner.IsError)
            return isOwner.Errors;

        var result = category.Activate();
        if (result.IsError) return result.Errors;

        var items = await _context.TbItems.Where(i => i.CategoryId == category.Id).ToListAsync();

        foreach (var item in items)
        {
            item.Activate();
        }

        await _context.SaveChangesAsync(cancellation);
        return Result.Updated;
    }
}
