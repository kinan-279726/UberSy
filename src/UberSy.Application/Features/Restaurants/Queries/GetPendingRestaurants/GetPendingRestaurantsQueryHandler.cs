using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Restaurants.Dtos;
using UberSy.Application.Features.Restaurants.Mappers;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Queries.GetPendingRestaurants;

public sealed class GetPendingRestaurantsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetPendingRestaurantsQuery, Result<PagedList<RestaurantDto>>>
{
    public async Task<Result<PagedList<RestaurantDto>>> Handle(
        GetPendingRestaurantsQuery request, CancellationToken cancellationToken)
    {
        var query = context.TbRestaurants.AsNoTracking().Where(r => !r.IsApproved);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(r => r.CreatedAtUtc)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return PagedList<RestaurantDto>.Create(items.ToDtos(), request.PageNumber, request.PageSize, totalCount);
    }
}