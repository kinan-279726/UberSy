using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Menu.Categories.Dtos;
using UberSy.Application.Features.Restaurants.Menu.Categories.Mappers;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Categories.Quiries.GetCategoriesByRestaurant;

public sealed class GetCategoriesByRestaurantQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetCategoriesByRestaurantQuery, Result<List<CategoryDto>>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesByRestaurantQuery request, CancellationToken cancellationToken)
    {
        var categories = await _context.TbCategories.AsNoTracking()
            .Where(c => c.RestaurantId == request.RestaurantId)
            .OrderBy(s => s.SortOrder)
            .ToListAsync(cancellationToken);

        return categories.ToDtos();
    }
}