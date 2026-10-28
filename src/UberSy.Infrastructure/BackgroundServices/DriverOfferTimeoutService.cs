using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.ServiceRequests.Offers.Enums;
using UberSy.Infrastructure.Data;

namespace UberSy.Infrastructure.BackgroundServices;

public sealed class DriverOfferTimeoutService(
    IServiceScopeFactory scopeFactory,
    ILogger<DriverOfferTimeoutService> logger)
    : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
    private readonly ILogger<DriverOfferTimeoutService> _logger = logger;
    private static readonly TimeSpan CheckInterval = TimeSpan.FromSeconds(15);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiredOffersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error While Processing Expired Driver Offers.");
            }

            await Task.Delay(CheckInterval, stoppingToken);
        }
    }

    private async Task ProcessExpiredOffersAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var dispatchService = scope.ServiceProvider.GetRequiredService<IDriverDispatchService>();

        var expiredOffers = await context.TbServiceRequestOffers
            .Where(o => o.Status == OfferStatus.Sent && o.ResponseTimeout <= DateTime.UtcNow)
            .ToListAsync(ct);

        if (expiredOffers.Count == 0)
        {
            return;
        }

        var affectedServiceRequestIds = expiredOffers.Select(o => o.ServiceRequestId).Distinct().ToList();

        foreach (var offer in expiredOffers)
        {
            var result = offer.Expire();

            if (result.IsError)
            {
                _logger.LogWarning("Failed To Expire Offer '{Id}': {Error}", offer.Id, result.TopError.Description);
            }
        }

        await context.SaveChangesAsync(ct);

        foreach (var serviceRequestId in affectedServiceRequestIds)
        {
            var stillHasPendingOffers = await context.TbServiceRequestOffers
                .AnyAsync(o => o.ServiceRequestId == serviceRequestId && o.Status == OfferStatus.Sent, ct);

            if (stillHasPendingOffers)
            {
                continue;
            }

            var foodOrder = await context.TbFoodOrders
                .FirstOrDefaultAsync(o => o.Id == serviceRequestId, ct);

            if (foodOrder is null || foodOrder.DriverId is not null)
            {
                continue;
            }

            var dispatchResult = await dispatchService.DispatchNextBatchAsync(serviceRequestId, ct);

            if (dispatchResult.IsError)
            {
                _logger.LogWarning("Could Not Dispatch Next Batch For '{Id}': {Error}",
                    serviceRequestId, dispatchResult.TopError.Description);

            }
        }
    }
}