using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Restaurants.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Queries.GetAllRestaurant;

public sealed class GetAllRestaurantQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetAllRestaurantQuery, Result<PagedList<RestaurantListItemDto>>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<PagedList<RestaurantListItemDto>>> Handle(
        GetAllRestaurantQuery request, CancellationToken cancellationToken)
    {
        var query = _context.TbRestaurants
            .AsNoTracking()
            .Where(r => r.IsApproved);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(r => r.NameAr.Contains(search) || r.NameEn.Contains(search));
        }

        if (request.IsOpenNow.HasValue)
        {
            query = query.Where(r => r.IsOpenNow == request.IsOpenNow.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.IsOpenNow)
            .ThenByDescending(r => r.Rating)
            .ThenBy(r => r.NameAr)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new RestaurantListItemDto
            {
                Id = r.Id,
                NameAr = r.NameAr,
                NameEn = r.NameEn,
                LogoUrl = r.LogoUrl,
                IsOpenNow = r.IsOpenNow,
                Rating = r.Rating,
            })
            .ToListAsync(cancellationToken);

        return PagedList<RestaurantListItemDto>.Create(items, request.PageNumber, request.PageSize, totalCount);
    }
}
