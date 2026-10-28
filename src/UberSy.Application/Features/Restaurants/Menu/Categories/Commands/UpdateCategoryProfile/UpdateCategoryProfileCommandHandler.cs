using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Commands.UpdateCategoryProfile;

public sealed class UpdateCategoryProfileCommandHandler(IApplicationDbContext context, IOwnershipValidator ownership)
    : IRequestHandler<UpdateCategoryProfileCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<Updated>> Handle(UpdateCategoryProfileCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.TbCategories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (category is null)
        {
            return ApplicationErrors.CategoryNotFound;
        }

        var isOwner = await _ownership.EnsureIsOwnerByRestaurantIdAsync(category.RestaurantId, cancellationToken);
        if (isOwner.IsError)
            return isOwner.Errors;

        var result = category.UpdateProfile(request.NameAr, request.NameEn);

        if (result.IsError)
        {
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}