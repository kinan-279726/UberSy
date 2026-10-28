using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetMyOrders;

public sealed record GetMyOrdersQuery(ServiceRequestStatus? Status, int PageNumber = 1, int PageSize = 20)
    : IRequest<Result<PagedList<CustomerOrderListItemDto>>>;

// for custormer 
public sealed class GetMyOrdersQueryHandler(IApplicationDbContext context, IOwnershipValidator ownership)
    : IRequestHandler<GetMyOrdersQuery, Result<PagedList<CustomerOrderListItemDto>>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<PagedList<CustomerOrderListItemDto>>> Handle(
        GetMyOrdersQuery request, CancellationToken cancellationToken)
    {
        var userResult = await _ownership.IsUser();
        if (userResult.IsError) return userResult.Errors;
        var userId = userResult.Value;

        var query = _context.TbFoodOrders.AsNoTracking()
            .Where(o => o.IdNavigation.CustomerId == userId);

        if (request.Status.HasValue)
            query = query.Where(o => o.IdNavigation.Status == request.Status.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(o => o.IdNavigation.CreatedAtUtc)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new CustomerOrderListItemDto(
                o.Id, o.IdNavigation.TrackingNumber, o.IdNavigation.Status,
                o.IdNavigation.TotalAmount, o.DeliveryFee, o.IdNavigation.TotalAmount + o.DeliveryFee,
                o.RestaurantId, o.Restaurant.NameAr, o.Restaurant.NameEn, o.Restaurant.LogoUrl,
                o.OrderItems.Count, o.IdNavigation.CreatedAtUtc))
            .ToListAsync(cancellationToken);

        return PagedList<CustomerOrderListItemDto>.Create(items, request.PageNumber, request.PageSize, total);
    }
}