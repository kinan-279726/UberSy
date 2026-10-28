using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Commands.RejectFoodOrder;

public sealed class RejectOrderCommandHandler(
    IOwnershipValidator ownership,
    IApplicationDbContext context,
    IUserNotifier notifier,
    ILogger<RejectOrderCommandHandler> logger)
    : IRequestHandler<RejectOrderCommand, Result<Updated>>
{
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly IApplicationDbContext _context = context;
    private readonly IUserNotifier _notifier = notifier;
    private readonly ILogger<RejectOrderCommandHandler> _logger = logger;

    public async Task<Result<Updated>> Handle(RejectOrderCommand request, CancellationToken cancellationToken)
    {
        var foodOrder = await _context.TbFoodOrders
            .Include(o => o.IdNavigation)
            .Include(o => o.Restaurant)
            .FirstOrDefaultAsync(o => o.Id == request.ServiceRequestId, cancellationToken);

        if (foodOrder is null)
        {
            return ApplicationErrors.ServiceRequestNotFound;
        }

        var ownerResult = await _ownership.EnsureIsOwnerByOwnerIdAsync(foodOrder.Restaurant.OwnerId, cancellationToken);
        if (ownerResult.IsError)
        {
            return ownerResult.Errors;
        }

        var result = foodOrder.IdNavigation.Cancel(request.Reason);

        if (result.IsError)
        {
            _logger.LogWarning("Failed To Reject Order '{Id}': {Error}", request.ServiceRequestId, result.TopError.Description);
            return result.Errors;
        }

        await _context.SaveChangesAsync(cancellationToken);

        await _notifier.NotifyServiceCancelledRestaurant(request.ServiceRequestId, foodOrder.IdNavigation.CustomerId, cancellationToken);

        _logger.LogInformation("Order '{Id}' Rejected By Restaurant '{RestaurantId}'. Reason: {Reason}",
            request.ServiceRequestId, foodOrder.RestaurantId, request.Reason);

        return Result.Updated;
    }
}