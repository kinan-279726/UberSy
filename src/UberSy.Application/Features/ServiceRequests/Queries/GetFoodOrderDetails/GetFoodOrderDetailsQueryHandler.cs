using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetFoodOrderDetails;

public sealed class GetFoodOrderDetailsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetFoodOrderDetailsQuery, Result<FoodOrderDetailsDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<Result<FoodOrderDetailsDto>> Handle(GetFoodOrderDetailsQuery query, CancellationToken ct)
    {
        var foodOrder = await _context.TbFoodOrders
            .AsNoTracking()
            .Include(o => o.IdNavigation)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Item)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.AddOns).ThenInclude(a => a.AddOn)
            .FirstOrDefaultAsync(o => o.Id == query.ServiceRequestId, ct);

        if (foodOrder is null)
        {
            return ApplicationErrors.ServiceRequestNotFound;
        }

        return new FoodOrderDetailsDto
        {
            ServiceRequestId = foodOrder.Id,
            TrackingNumber = foodOrder.IdNavigation.TrackingNumber,
            Status = (byte)foodOrder.IdNavigation.Status,
            TotalAmount = foodOrder.IdNavigation.TotalAmount,
            ResponseDeadlineUtc = foodOrder.ResponseTimeout,
            Items = foodOrder.OrderItems.Select(oi => new FoodOrderLineDto
            {
                ItemNameAr = oi.Item.NameAr,
                ItemNameEn = oi.Item.NameEn,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                Note = oi.Note,
                AddOnNamesAr = oi.AddOns.Select(a => a.AddOn.NameAr).ToList(),
            }).ToList(),
        };
    }
}