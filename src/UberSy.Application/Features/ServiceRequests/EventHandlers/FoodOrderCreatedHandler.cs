using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.ServiceRequests.Events.CreationEvents;

namespace UberSy.Application.Features.ServiceRequests.EventHandlers;
public sealed class FoodOrderCreatedHandler(IApplicationDbContext context ,
    IRestaurantNotifier notifier,
    ILogger<FoodOrderCreatedHandler> logger)
    : INotificationHandler<FoodOrderCreated>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IRestaurantNotifier _notifier = notifier;
    private readonly ILogger<FoodOrderCreatedHandler> _logger = logger;
    public async Task Handle(FoodOrderCreated notification, CancellationToken ct)
    {
        var foodOrder = await _context.TbFoodOrders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == notification.ServiceRequestId, ct);

        if (foodOrder is null)
        {
            _logger.LogWarning("FoodOrder '{Id}' Not Found When Handling FoodOrderCreated.", notification.ServiceRequestId);
            return;
        }

        await _notifier.NotifyNewOrderAsync(foodOrder.RestaurantId, notification.ServiceRequestId, ct);

        _logger.LogInformation("Restaurant '{RestaurantId}' Notified For New Order '{ServiceRequestId}'.",
            foodOrder.RestaurantId, notification.ServiceRequestId);
    }
}
