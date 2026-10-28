using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Menu.Items.Dtos;
using UberSy.Application.Features.Restaurants.Menu.Items.Mappers;
using UberSy.Domain.Common.Results;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Queries.GetItemsByCategory;

public sealed class GetItemsByCategoryQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetItemsByCategoryQuery, Result<List<ItemDto>>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<List<ItemDto>>> Handle(GetItemsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var categoryExists = await _context.TbCategories.AsNoTracking().AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (!categoryExists)
        {
            return ApplicationErrors.CategoryNotFound;
        }

        var items = await _context.TbItems
            .AsNoTracking()
            .Where(i => i.CategoryId == request.CategoryId && i.IsActive)
            .OrderBy(o => o.SortOrder)
            .ToListAsync(cancellationToken);

        return items.ToDtos();
    }
}