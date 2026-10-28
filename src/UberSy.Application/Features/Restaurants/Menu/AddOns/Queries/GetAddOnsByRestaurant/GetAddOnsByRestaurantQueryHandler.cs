using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Dtos;
using UberSy.Application.Features.Restaurants.Menu.AddOns.Mappers;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Menu.AddOns.Queries.GetAddOnsByRestaurant;

public sealed class GetAddOnsByRestaurantQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAddOnsByRestaurantQuery, Result<List<AddOnDto>>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<List<AddOnDto>>> Handle(GetAddOnsByRestaurantQuery request, CancellationToken cancellationToken)
    {
        var addOns = await _context.TbAddOns
            .AsNoTracking()
            .Where(a => a.RestaurantId == request.RestaurantId && a.IsActive)
            .Select(a => a.ToDto())
            .ToListAsync(cancellationToken);

        return addOns;
    }
}
