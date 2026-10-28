using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Infrastructure.Data;

namespace UberSy.Infrastructure.BackgroundServices;

public sealed class OrderResponseTimeoutService(IServiceScopeFactory scopeFactory,
    ILogger<OrderResponseTimeoutService> logger)
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<OrderResponseTimeoutService> _logger = logger;
    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(30);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessTimedOutOrdersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error While Processing Timed-Out Orders.");
            }

            await Task.Delay(CheckInterval, stoppingToken);
        }
    }

    private async Task ProcessTimedOutOrdersAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var notifier = scope.ServiceProvider.GetRequiredService<IUserNotifier>();

        var timedOutOrders = await context.TbFoodOrders
            .Include(o => o.IdNavigation)
            .Where(o => o.ResponseTimeout <= DateTime.UtcNow)
            .Where(o => o.IdNavigation.Status == ServiceRequestStatus.Pending)
            .ToListAsync(stoppingToken);

        foreach (var order in timedOutOrders)
        {
            var result = order.IdNavigation.Cancel("لم يرد المطعم خلال المهلة المحددة");

            if (result.IsError)
            {
                _logger.LogWarning("Failed To Auto-Cancel Order '{Id}': {Error}", order.Id, result.TopError.Description);
                continue;
            }

           await notifier.NotifyServiceCancelledBySystem(order.Id, order.IdNavigation.CustomerId, stoppingToken);

            _logger.LogInformation("Order '{Id}' Auto-Cancelled Due To Restaurant Timeout.", order.Id);
        }

        if (timedOutOrders.Count > 0)
        {
            await context.SaveChangesAsync(stoppingToken);
        }
    }
}
