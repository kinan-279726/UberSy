using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetRestaurantOrders;

public sealed class GetRestaurantOrdersQueryHandler(
    IApplicationDbContext context,
    IOwnershipValidator ownershipValidator)
    : IRequestHandler<GetRestaurantOrdersQuery, Result<PagedList<RestaurantOrderListItemDto>>>
{
    public async Task<Result<PagedList<RestaurantOrderListItemDto>>> Handle(
        GetRestaurantOrdersQuery request, CancellationToken cancellationToken)
    {
        var ownership = await ownershipValidator
            .EnsureIsOwnerByRestaurantIdAsync(request.RestaurantId, cancellationToken);
        if (ownership.IsError) return ownership.Errors;

        var query = context.TbFoodOrders
            .AsNoTracking()
            .Where(o => o.RestaurantId == request.RestaurantId);

        if (request.Status.HasValue)
            query = query.Where(o => o.IdNavigation.Status == request.Status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.IdNavigation.CreatedAtUtc)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new RestaurantOrderListItemDto(
                o.Id,
                o.IdNavigation.TrackingNumber,
                o.IdNavigation.Status,
                o.IdNavigation.TotalAmount,
                o.OrderItems.Count,
                o.IdNavigation.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return PagedList<RestaurantOrderListItemDto>.Create(items, request.PageNumber, request.PageSize, totalCount);
    }
}