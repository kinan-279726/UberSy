using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Restaurants.Dtos;
using UberSy.Application.Features.Restaurants.Mappers;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Application.Features.Restaurants.Queries.GetPendingRestaurants;

public sealed class GetPendingRestaurantsQueryHandler(IApplicationDbContext context , IUser user)
    : IRequestHandler<GetPendingRestaurantsQuery, Result<PagedList<RestaurantDto>>>
{
    private readonly IUser _user = user;
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<PagedList<RestaurantDto>>> Handle(
        GetPendingRestaurantsQuery request, CancellationToken cancellationToken)
    {
        if (_user.Id is null)
        {
            return ApplicationErrors.Unauthorized;
        }
        if (!_user.IsInRole(Roles.Admin))
        {
            return ApplicationErrors.AccessDenied;
        }

        var query = _context.TbRestaurants.AsNoTracking().Where(r => !r.IsApproved);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(r => r.CreatedAtUtc)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return PagedList<RestaurantDto>.Create(items.ToDtos(), request.PageNumber, request.PageSize, totalCount);
    }
}
