using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Restaurants.Dtos;
using UberSy.Application.Features.Restaurants.Mappers;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Restaurants.Queries.GetRestaurantById;

public sealed class GetRestaurantByIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetRestaurantByIdQuery, Result<RestaurantDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<RestaurantDto>> Handle(GetRestaurantByIdQuery request, CancellationToken cancellationToken)
    {
        var restaurant = await _context.TbRestaurants
            .AsNoTracking()
            .Include(i => i.Images)
            .Include(c => c.Categories)
            .ThenInclude(i => i.Items)
            .FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);

        if (restaurant is null)
        {
            return ApplicationErrors.RestaurantNotFound;
        }

        return restaurant.ToDto();
    }
}
