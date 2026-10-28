using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.ServiceRequests.Events.DriverEvents;
using UberSy.Domain.ServiceRequests.Offers.Enums;

namespace UberSy.Application.Features.ServiceRequests.EventHandlers;

public sealed class DriverAcceptedOfferHandler(
    IApplicationDbContext context,
    IDriverNotifier notifier,
    IDeliveryPricingService pricing,
    ILogger<DriverAcceptedOfferHandler> logger) : INotificationHandler<DriverAcceptedOffer>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IDriverNotifier _notifier = notifier;
    private readonly IDeliveryPricingService _pricing = pricing;
    private readonly ILogger<DriverAcceptedOfferHandler> _logger = logger;

    public async Task Handle(DriverAcceptedOffer notification, CancellationToken cancellationToken)
    {
        var foodOrder = await _context.TbFoodOrders
            .Include(o => o.Restaurant)
            .Include(o => o.Address)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.AddOns)
            .FirstOrDefaultAsync(o => o.Id == notification.ServiceRequestId, cancellationToken);

        if (foodOrder is null)
        {
            _logger.LogWarning("FoodOrder '{Id}' Not Found When Handling DriverAcceptedOffer.", notification.ServiceRequestId);
            return;
        }

        if (foodOrder.DriverId is not null)
        {
            return;
        }

        var driver = await _context.TbDrivers
            .FirstAsync(d => d.Id == notification.DriverId, cancellationToken);

        var quoteResult = await _pricing.CalculateAsync(
            foodOrder.Restaurant.Boundary,
            foodOrder.Address.Boundary,
            driver.VehicleType,
            driver.CarType,
            cancellationToken);

        if (quoteResult.IsError)
        {
            _logger.LogError("Pricing Failed For Order '{Id}' And Driver '{DriverId}': {Error}",
                foodOrder.Id, driver.Id, quoteResult.TopError.Description);
            return;
        }

        var quote = quoteResult.Value;

        var itemsSubtotal = foodOrder.OrderItems
            .Sum(oi => (oi.UnitPrice + oi.AddOns.Sum(a => a.Price)) * oi.Quantity);

        var commission = Math.Round(itemsSubtotal * foodOrder.Restaurant.CommissionRate / 100m, 2);

        var assignResult = foodOrder.AssignDriver(driver.Id);
        if (assignResult.IsError)
        {
            _logger.LogError("Failed To Assign Driver '{DriverId}' To FoodOrder '{Id}': {Error}",
                driver.Id, foodOrder.Id, assignResult.TopError.Description);
            return;
        }

        var financialsResult = foodOrder.RecordDeliveryFinancials(
            deliveryFee: quote.CustomerFee,
            driverEarning: quote.DriverEarning,
            platformCommission: commission);

        if (financialsResult.IsError)
        {
            _logger.LogError("Order '{Id}' Failed To Record Financials: {Error}",
                foodOrder.Id, financialsResult.TopError.Description);
            foodOrder.RemoveDriver();
            return;
        }

        var startResult = driver.StartDelivery();
        if (startResult.IsError)
        {
            _logger.LogWarning("Could Not Set Driver '{DriverId}' OnDelivery: {Error}",
                driver.Id, startResult.TopError.Description);
        }

        var otherPendingOffers = await _context.TbServiceRequestOffers
            .Where(o => o.ServiceRequestId == notification.ServiceRequestId)
            .Where(o => o.Id != notification.OfferId)
            .Where(o => o.Status == OfferStatus.Sent)
            .ToListAsync(cancellationToken);

        foreach (var offer in otherPendingOffers)
        {
            offer.MarkAsLostToAnotherDriver();
        }

        await _context.SaveChangesAsync(cancellationToken);

        foreach (var offer in otherPendingOffers)
        {
            await _notifier.NotifyOfferLostAsync(offer.DriverId, offer.Id, offer.ServiceRequestId, cancellationToken);
        }

        _logger.LogInformation(
            "Driver '{DriverId}' Assigned To FoodOrder '{Id}' (Fee {Fee}, Earning {Earning}, Commission {Commission}). {Count} Other Offers Marked As Lost.",
            driver.Id, foodOrder.Id, quote.CustomerFee, quote.DriverEarning, commission, otherPendingOffers.Count);
    }
}