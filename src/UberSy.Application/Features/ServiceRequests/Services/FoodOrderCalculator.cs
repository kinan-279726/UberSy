using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.ServiceRequests.Commands.PlaceFoodOrder;
using UberSy.Domain.Common.Results;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Restaurants.AddOns;

namespace UberSy.Application.Features.ServiceRequests.Services;

public sealed class FoodOrderCalculator(
    IApplicationDbContext context,
    IDeliveryPricingService pricing) : IFoodOrderCalculator
{
    private readonly IApplicationDbContext _context = context;
    private readonly IDeliveryPricingService _pricing = pricing;

    public async Task<Result<FoodOrderCalculation>> CalculateAsync(
        Guid customerId,
        Guid restaurantId,
        Guid addressId,
        IReadOnlyCollection<PlaceFoodOrderItem> items,
        CancellationToken cancellationToken)
    {
        var restaurant = await _context.TbRestaurants
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == restaurantId, cancellationToken);

        if (restaurant is null) return ApplicationErrors.RestaurantNotFound;
        if (!restaurant.IsApproved) return ApplicationErrors.RestaurantNotApproved;
        if (!restaurant.IsOpenNow) return ApplicationErrors.RestaurantClosed;

        var address = await _context.TbAddresses
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == customerId, cancellationToken);

        if (address is null) return ApplicationErrors.AddressNotFound;

        var requestedItemIds = items.Select(i => i.ItemId).Distinct().ToList();

        var dbItems = await _context.TbItems
            .AsNoTracking()
            .Include(i => i.AddOns)
            .Where(i => requestedItemIds.Contains(i.Id))
            .ToListAsync(cancellationToken);

        if (dbItems.Count != requestedItemIds.Count) return ApplicationErrors.ItemNotFound;

        if (dbItems.Any(i => i.RestaurantId != restaurantId || !i.IsActive))
        {
            return ApplicationErrors.ItemNotInRestaurant;
        }

        var itemsById = dbItems.ToDictionary(i => i.Id);
        var lines = new List<FoodOrderLineCalculation>();
        decimal itemsTotal = 0;

        foreach (var requested in items)
        {
            var item = itemsById[requested.ItemId];
            var addOns = new List<TbAddOn>();

            foreach (var addOnId in requested.AddOnIds.Distinct())
            {
                var addOn = item.AddOns.FirstOrDefault(a => a.Id == addOnId);

                if (addOn is null) return ApplicationErrors.AddOnNotAvailableForItem;

                if (addOn.RestaurantId != restaurantId || !addOn.IsActive)
                {
                    return ApplicationErrors.AddOnNotInRestaurant;
                }

                addOns.Add(addOn);
            }

            var lineTotal = (item.Price + addOns.Sum(a => a.Price)) * requested.Quantity;
            itemsTotal += lineTotal;

            lines.Add(new FoodOrderLineCalculation
            {
                Item = item,
                Quantity = requested.Quantity,
                AddOns = addOns,
                LineTotal = lineTotal,
            });
        }

        if (itemsTotal < restaurant.MinOrderAmount) return ApplicationErrors.BelowMinimumOrderAmount;
        if (itemsTotal > restaurant.MaxOrderAmount) return ApplicationErrors.AboveMaximumOrderAmount;

        const decimal discountAmount = 0m;

        var taxable = itemsTotal - discountAmount;
        var taxAmount = Math.Round(taxable * restaurant.TaxRate / 100m, 2);
        var totalAmount = taxable + taxAmount;

        var delivery = await _pricing.CalculateAsync(
            restaurant.Boundary, address.Boundary, VehicleType.Motorcycle, null, cancellationToken);

        if (delivery.IsError) return delivery.Errors;

        return new FoodOrderCalculation
        {
            Restaurant = restaurant,
            Lines = lines,
            ItemsTotal = itemsTotal,
            DiscountAmount = discountAmount,
            TaxAmount = taxAmount,
            TotalAmount = totalAmount,
            Delivery = delivery.Value,
        };
    }
}