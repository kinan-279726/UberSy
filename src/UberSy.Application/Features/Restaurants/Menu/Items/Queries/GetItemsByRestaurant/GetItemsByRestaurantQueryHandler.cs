using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Menu.Items.Dtos;
using UberSy.Application.Features.Restaurants.Menu.Items.Mappers;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.Items.Queries.GetItemsByRestaurant;

public sealed class GetItemsByRestaurantQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetItemsByRestaurantQuery, Result<List<ItemDto>>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<List<ItemDto>>> Handle(GetItemsByRestaurantQuery request, CancellationToken cancellationToken)
    {
        var items = await _context.TbItems
            .AsNoTracking()
            .Where(i => i.RestaurantId == request.RestaurantId && i.IsActive)
            .OrderBy(o => o.SortOrder)
            .ToListAsync(cancellationToken);

        return items.ToDtos();
    }
}