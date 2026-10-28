using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Index.HPRtree;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Constants;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Commands.UpdateCategorySortOrder;

public sealed class UpdateCategorySortOrderCommandHandler(IApplicationDbContext context, IOwnershipValidator ownership)
    : IRequestHandler<UpdateCategorySortOrderCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<Updated>> Handle(UpdateCategorySortOrderCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.TbCategories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (category is null) return ApplicationErrors.CategoryNotFound;

        var isOwner = await _ownership.EnsureIsOwnerByRestaurantIdAsync(category.RestaurantId, cancellationToken);
        if (isOwner.IsError)
            return isOwner.Errors;

        var result = category.UpdateSortOrder(request.SortOrder);
        if (result.IsError) return result.Errors;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    } 
}