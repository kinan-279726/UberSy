using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.RemovePrinterFromCategory;
using UberSy.Domain.Common.Results;

public sealed class RemovePrinterFromCategoryCommandHandler(IApplicationDbContext context , IOwnershipValidator ownership)
    : IRequestHandler<RemovePrinterFromCategoryCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<Updated>> Handle(RemovePrinterFromCategoryCommand request, CancellationToken cancellation)
    {
        var category = await _context.TbCategories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellation);

        if(category is not null)
        {
            var result = await _ownership.EnsureIsOwnerByRestaurantIdAsync(category.RestaurantId, cancellation);
            if (result.IsError) return result.Errors;
        }
        else  return ApplicationErrors.CategoryNotFound;

        var resultCat = category.RemovePrinter();
        if (resultCat.IsError) return resultCat.Errors;

        await _context.SaveChangesAsync(cancellation);
        return Result.Updated;
    }
}
