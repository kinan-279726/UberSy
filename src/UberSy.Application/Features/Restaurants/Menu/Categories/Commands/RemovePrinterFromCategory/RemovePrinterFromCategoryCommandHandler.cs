using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Menu.Categories.Commands.RemovePrinterFromCategory;
using UberSy.Domain.Common.Results;

public sealed class RemovePrinterFromCategoryCommandHandler(IApplicationDbContext context)
    : IRequestHandler<RemovePrinterFromCategoryCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<Updated>> Handle(RemovePrinterFromCategoryCommand request, CancellationToken cancellation)
    {
        var category = await _context.TbCategories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellation);
        if (category is null) return ApplicationErrors.CategoryNotFound;

        var result = category.RemovePrinter();
        if (result.IsError) return result.Errors;

        await _context.SaveChangesAsync(cancellation);
        return Result.Updated;
    }
}