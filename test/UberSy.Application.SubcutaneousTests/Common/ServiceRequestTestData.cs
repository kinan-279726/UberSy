using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.SubcutaneousTests.Common.Factories;
using UberSy.Domain.Common.Results;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Drivers;
using UberSy.Domain.Drivers.Wallet;
using UberSy.Domain.Restaurants;
using UberSy.Domain.Restaurants.AddOns;
using UberSy.Domain.Restaurants.Categories;
using UberSy.Domain.Restaurants.Items;
using UberSy.Domain.ServiceRequests;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Domain.ServiceRequests.FoodOrders;
using UberSy.Domain.ServiceRequests.Offers;
using UberSy.Domain.ServiceRequests.Offers.Enums;
using UberSy.Test.Common.Security;
using UberSy.Test.Common.ServiceRequest;

namespace UberSy.Application.SubcutaneousTests.Common;

public sealed record SeededMenu(TbRestaurant Restaurant, TbCategory Category, TbItem Item, TbAddOn? AddOn);

public sealed record SeededOrder(
    Guid Id,
    Guid CustomerId,
    Guid RestaurantId,
    Guid AddressId,
    Guid ItemId,
    Guid OrderItemId,
    decimal ItemsTotal,
    decimal TotalAmount);

/// <summary>
/// Persists service requests, food orders, offers and dispatchable drivers directly through the db context.
/// Domain events raised while building the data are cleared, so seeding never triggers the event handlers
/// (no dispatch, no notifications). Users used here must be seeded in WebAppFactory.
/// </summary>
public static class ServiceRequestTestData
{
    // ---------- menu ----------

    public static async Task<SeededMenu> SeedMenuAsync(
        IApplicationDbContext context,
        Guid? ownerId = null,
        bool approved = true,
        bool open = true,
        decimal itemPrice = 25m,
        bool withAddOn = false,
        decimal addOnPrice = 5m)
    {
        var restaurant = await RestaurantTestData.SeedRestaurantAsync(
            context, ownerId ?? TestUsers.Owner1.Id, approved: approved, open: open);

        var category = await RestaurantTestData.SeedCategoryAsync(context, restaurant.Id);
        var item = await RestaurantTestData.SeedItemAsync(context, restaurant.Id, category.Id, price: itemPrice);

        TbAddOn? addOn = null;
        if (withAddOn)
        {
            addOn = await RestaurantTestData.SeedAddOnAsync(context, restaurant.Id, price: addOnPrice);
            await RestaurantTestData.AssignAddOnAsync(context, item, addOn);
        }

        return new SeededMenu(restaurant, category, item, addOn);
    }

    // Adds another active item to an existing restaurant
    public static async Task<TbItem> SeedExtraItemAsync(
        IApplicationDbContext context, SeededMenu menu, decimal price = 40m, byte sortOrder = 2)
        => await RestaurantTestData.SeedItemAsync(context, menu.Restaurant.Id, menu.Category.Id, price: price, sortOrder: sortOrder);

    // ---------- customer address ----------

    public static async Task<Guid> SeedAddressAsync(IApplicationDbContext context, Guid? userId = null)
    {
        var address = AddressFactory.CreateValidAddress(userId: userId ?? TestUsers.Customer.Id);
        context.TbAddresses.Add(address);
        await context.SaveChangesAsync(CancellationToken.None);
        return address.Id;
    }

    // ---------- orders ----------

    /// <summary>
    /// Seeds address + service request + food order + one order line (optionally with the menu add-on).
    /// Totals follow the real calculation: items total + restaurant tax (5%). Delivery fee is NOT part of TotalAmount.
    /// </summary>
    public static async Task<SeededOrder> SeedOrderAsync(
        IApplicationDbContext context,
        SeededMenu menu,
        Guid? customerId = null,
        ServiceRequestStatus status = ServiceRequestStatus.Pending,
        Guid? driverId = null,
        int quantity = 2,
        bool includeAddOn = false,
        decimal deliveryFee = 0m,
        decimal driverEarning = 0m,
        decimal commission = 0m,
        DateTimeOffset? responseTimeout = null,
        string? note = null)
    {
        var customer = customerId ?? TestUsers.Customer.Id;
        var address = AddressFactory.CreateValidAddress(userId: customer);

        var unitPrice = menu.Item.Price;
        var addOnPrice = includeAddOn && menu.AddOn is not null ? menu.AddOn.Price : 0m;
        var itemsTotal = (unitPrice + addOnPrice) * quantity;
        var tax = Math.Round(itemsTotal * menu.Restaurant.TaxRate / 100m, 2);
        var total = itemsTotal + tax;

        var request = Unwrap(ServiceRequestFactory.CreateServiceRequest(
            customerId: customer, serviceType: ServiceType.Food, totalAmount: total));

        ApplyStatus(request, status);

        var order = Unwrap(FoodOrderFactory.CreateFoodOrder(
            id: request.Id,
            restaurantId: menu.Restaurant.Id,
            addressId: address.Id,
            responseTimeout: responseTimeout ?? DateTimeOffset.UtcNow.AddMinutes(10)));

        if (driverId is not null) Unwrap(order.AssignDriver(driverId.Value));

        if (deliveryFee > 0 || driverEarning > 0 || commission > 0)
            Unwrap(order.RecordDeliveryFinancials(deliveryFee, driverEarning, commission));

        // Seeding must not trigger the application event handlers
        request.ClearDomainEvents();
        order.ClearDomainEvents();

        context.TbAddresses.Add(address);
        context.TbServiceRequests.Add(request);
        context.TbFoodOrders.Add(order);

        var orderItem = Unwrap(TbOrderItem.Create(Guid.NewGuid(), order.Id, menu.Item.Id, quantity, unitPrice, note));
        context.TbOrderItems.Add(orderItem);

        if (includeAddOn && menu.AddOn is not null)
        {
            context.TbOrderItemAddOns.Add(
                Unwrap(TbOrderItemAddOn.Create(Guid.NewGuid(), orderItem.Id, menu.AddOn.Id, addOnPrice)));
        }

        await context.SaveChangesAsync(CancellationToken.None);

        return new SeededOrder(order.Id, customer, menu.Restaurant.Id, address.Id, menu.Item.Id, orderItem.Id, itemsTotal, total);
    }

    /// <summary>Moves the stored CreatedAtUtc of a service request (the interceptor always stamps "now" on insert).</summary>
    public static async Task BackdateAsync(IApplicationDbContext context, Guid serviceRequestId, DateTimeOffset createdAtUtc)
    {
        await context.TbServiceRequests
            .Where(s => s.Id == serviceRequestId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.CreatedAtUtc, createdAtUtc));
    }

    // ---------- drivers ----------

    /// <summary>
    /// Seeds an approved driver that the dispatcher can pick: Online, wallet balance &gt; 0 and a stored location.
    /// </summary>
    public static async Task<TbDriver> SeedDispatchableDriverAsync(
        IApplicationDbContext context,
        Guid userId,
        double latitude = 33.5138,
        double longitude = 36.2765,
        decimal walletBalance = 100m,
        bool online = true,
        bool onDelivery = false,
        VehicleType vehicleType = VehicleType.Motorcycle,
        bool withLocation = true)
    {
        CarType? carType = vehicleType == VehicleType.Car ? CarType.Private : null;

        var driver = Unwrap(TbDriver.Create(
            id: userId,
            userId: userId,
            vehicleType: vehicleType,
            serviceTier: ServiceTier.Standard,
            carType: carType,
            nationalId: Guid.NewGuid().ToString("N")[..12],
            photoUrl: null,
            licenseNumber: vehicleType == VehicleType.Bicycle ? null : "LIC-12345",
            plateNumber: vehicleType == VehicleType.Bicycle ? null : "PLATE-1",
            model: "Honda",
            color: "Red"));

        Unwrap(driver.Approve());
        if (online || onDelivery) Unwrap(driver.GoOnline());
        if (onDelivery) Unwrap(driver.StartDelivery());

        context.TbDrivers.Add(driver);

        var wallet = Unwrap(TbDriverWallet.Create(Guid.NewGuid(), driver.Id));
        if (walletBalance > 0) Unwrap(wallet.Deposit(walletBalance));
        context.TbDriverWallets.Add(wallet);

        if (withLocation)
        {
            context.TbDriverLocations.Add(Unwrap(TbDriverLocation.Create(
                Guid.NewGuid(), driver.Id, new Point(longitude, latitude) { SRID = 4326 })));
        }

        await context.SaveChangesAsync(CancellationToken.None);
        return driver;
    }

    // ---------- offers ----------

    public static async Task<TbServiceRequestOffer> SeedOfferAsync(
        IApplicationDbContext context,
        Guid serviceRequestId,
        Guid driverId,
        OfferStatus status = OfferStatus.Sent,
        DateTimeOffset? responseTimeout = null)
    {
        var offer = Unwrap(TbServiceRequestOffer.Create(
            Guid.NewGuid(), serviceRequestId, driverId, responseTimeout ?? DateTimeOffset.UtcNow.AddMinutes(1)));

        switch (status)
        {
            case OfferStatus.Accepted: Unwrap(offer.Accept()); break;
            case OfferStatus.Rejected: Unwrap(offer.Reject()); break;
            case OfferStatus.Expired: Unwrap(offer.Expire()); break;
            case OfferStatus.LostToAnotherDriver: Unwrap(offer.MarkAsLostToAnotherDriver()); break;
        }

        offer.ClearDomainEvents();

        context.TbServiceRequestOffers.Add(offer);
        await context.SaveChangesAsync(CancellationToken.None);
        return offer;
    }

    // ---------- reading back ----------

    public static Task<TbFoodOrder> LoadOrderAsync(IApplicationDbContext context, Guid id)
        => context.TbFoodOrders.AsNoTracking().Include(o => o.IdNavigation).FirstAsync(o => o.Id == id);

    public static Task<TbDriver> LoadDriverAsync(IApplicationDbContext context, Guid id)
        => context.TbDrivers.AsNoTracking().FirstAsync(d => d.Id == id);

    public static Task<TbServiceRequestOffer> LoadOfferAsync(IApplicationDbContext context, Guid id)
        => context.TbServiceRequestOffers.AsNoTracking().FirstAsync(o => o.Id == id);

    // ---------- internals ----------

    private static void ApplyStatus(TbServiceRequest request, ServiceRequestStatus status)
    {
        switch (status)
        {
            case ServiceRequestStatus.Pending:
                break;
            case ServiceRequestStatus.Preparing:
                Unwrap(request.MarkAsPreparing());
                break;
            case ServiceRequestStatus.OutForDelivery:
                Unwrap(request.MarkAsPreparing());
                Unwrap(request.MarkAsOutForDelivery());
                break;
            case ServiceRequestStatus.Delivered:
                Unwrap(request.MarkAsPreparing());
                Unwrap(request.MarkAsOutForDelivery());
                Unwrap(request.MarkAsDelivered());
                break;
            case ServiceRequestStatus.Cancelled:
                Unwrap(request.Cancel("seeded"));
                break;
        }
    }

    private static T Unwrap<T>(Result<T> result)
        => result.IsError
            ? throw new InvalidOperationException($"Seeding failed: {result.TopError.Code} - {result.TopError.Description}")
            : result.Value;
}
