using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Commands.AcceptFoodOrder;

public sealed class AcceptOrderCommandHandler(
    IOwnershipValidator ownership,
    IApplicationDbContext context,
    ILogger<AcceptOrderCommandHandler> logger)
    : IRequestHandler<AcceptOrderCommand, Result<Updated>>
{
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<AcceptOrderCommandHandler> _logger = logger;

    public async Task<Result<Updated>> Handle(AcceptOrderCommand request, CancellationToken cancellation)
    {
        var foodOrder = await _context.TbFoodOrders
            .Include(o => o.IdNavigation)
            .Include(o => o.Restaurant)
            .FirstOrDefaultAsync(o => o.Id == request.ServiceRequestId, cancellation);

        if (foodOrder is null)
        {
            return ApplicationErrors.ServiceRequestNotFound;
        }

        var ownerResult = await _ownership.EnsureIsOwnerByOwnerIdAsync(foodOrder.Restaurant.OwnerId);
        if (ownerResult.IsError)
        {
            return ownerResult.Errors;
        }

        var result = foodOrder.IdNavigation.MarkAsPreparing();

        if (result.IsError)
        {
            _logger.LogWarning("Failed To Accept Order '{Id}': {Error}", request.ServiceRequestId, result.TopError.Description);
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellation);

        _logger.LogInformation("Order '{Id}' Accepted By Restaurant '{RestaurantId}'.", request.ServiceRequestId, foodOrder.RestaurantId);

        return Result.Updated;
    }
}