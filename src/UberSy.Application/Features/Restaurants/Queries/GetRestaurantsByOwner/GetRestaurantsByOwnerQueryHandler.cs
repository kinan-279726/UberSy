using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Dtos;
using UberSy.Application.Features.Restaurants.Mappers;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Queries.GetRestaurantsByOwner;

public sealed class GetRestaurantsByOwnerQueryHandler(IApplicationDbContext context, IOwnershipValidator ownership)
    : IRequestHandler<GetRestaurantsByOwnerQuery, Result<List<RestaurantDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<List<RestaurantDto>>> Handle(GetRestaurantsByOwnerQuery request, CancellationToken cancellationToken)
    {
        var userResult = await _ownership.IsUser();

        if (userResult.IsError) return userResult.Errors;

        var restaurants = await _context.TbRestaurants
            .AsNoTracking()
            .Where(r => r.OwnerId == userResult.Value)
            .Select(r => r.ToDto())
            .ToListAsync(cancellationToken);

        return restaurants;
    }
}