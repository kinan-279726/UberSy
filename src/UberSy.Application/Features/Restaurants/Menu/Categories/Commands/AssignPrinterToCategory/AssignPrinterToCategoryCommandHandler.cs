using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

public sealed class AssignPrinterToCategoryCommandHandler(IApplicationDbContext context)
    : IRequestHandler<AssignPrinterToCategoryCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Updated>> Handle(AssignPrinterToCategoryCommand request, CancellationToken cancellation)
    {
        var category = await _context.TbCategories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellation);
        if (category is null) return ApplicationErrors.CategoryNotFound;

        var printerBelongsToRestaurant = await _context.TbPrinters
            .AsNoTracking()
            .AnyAsync(p => p.Id == request.PrinterId && p.RestaurantId == category.RestaurantId, cancellation);

        if (!printerBelongsToRestaurant) return ApplicationErrors.PrinterNotFound;

        var result = category.AssignPrinter(request.PrinterId);
        if (result.IsError) return result.Errors;

        await _context.SaveChangesAsync(cancellation);
        return Result.Updated;
    }
}