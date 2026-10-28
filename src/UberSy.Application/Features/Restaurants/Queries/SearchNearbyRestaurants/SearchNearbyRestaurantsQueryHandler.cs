using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using NetTopologySuite.Geometries;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Dtos;
using UberSy.Application.Features.Restaurants.Mappers;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Queries.SearchNearbyRestaurants;

public sealed class SearchNearbyRestaurantsQueryHandler(IApplicationDbContext context , HybridCache cache)
    : IRequestHandler<SearchNearbyRestaurantsQuery, Result<List<RestaurantDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly HybridCache _cache = cache;

    public async Task<Result<List<RestaurantDto>>> Handle(SearchNearbyRestaurantsQuery request, CancellationToken cancellationToken)
    {
        var customerLocation = new Point(request.Longitude, request.Latitude) { SRID = 4326 };
        var radiusInMeters = request.RadiusKm * 1000;

        var restaurants = await _context.TbRestaurants
            .AsNoTracking()
            .Where(r => r.IsApproved && r.IsOpenNow)
            .Where(r => r.Boundary.Distance(customerLocation) <= radiusInMeters)
            .OrderBy(r => r.Boundary.Distance(customerLocation))
            .ToListAsync(cancellationToken);

        return restaurants.ToDtos();
    }
}