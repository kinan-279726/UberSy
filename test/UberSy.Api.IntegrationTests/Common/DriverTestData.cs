using System;
using System.Collections.Generic;
using System.Text;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.SubcutaneousTests.Common.Factories;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Drivers;
using UberSy.Domain.Drivers.Wallet;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Test.Common.Restaurants;
using UberSy.Test.Common.Security;
using UberSy.Test.Common.ServiceRequest;

namespace UberSy.Api.IntegrationTests.Common;

public static class DriverTestData
{
    // Seeds a motorcycle driver (+ optional wallet) directly into the database
    public static async Task<TbDriver> SeedDriverAsync(
        IApplicationDbContext context,
        Guid userId,
        bool approved = true,
        bool online = false,
        bool withWallet = true,
        string? nationalId = null)
    {
        var result = TbDriver.Create(
            id: Guid.NewGuid(),
            userId: userId,
            vehicleType: VehicleType.Motorcycle,
            serviceTier: ServiceTier.Standard,
            carType: null,
            nationalId: nationalId ?? Guid.NewGuid().ToString("N")[..12],
            photoUrl: null,
            licenseNumber: "LIC-12345",
            plateNumber: "PLATE-1",
            model: "Honda",
            color: "Red");

        if (result.IsError)
            throw new InvalidOperationException($"{result.TopError.Code}: {result.TopError.Description}");

        var driver = result.Value;

        if (approved) driver.Approve();
        if (online) driver.GoOnline();

        context.TbDrivers.Add(driver);

        if (withWallet)
            context.TbDriverWallets.Add(TbDriverWallet.Create(Guid.NewGuid(), driver.Id).Value);

        await context.SaveChangesAsync(CancellationToken.None);
        return driver;
    }


    // Seeds address + restaurant + service request + food order assigned to the given driver
    public static async Task<Guid> SeedFoodOrderAsync(
        IApplicationDbContext context,
        Guid driverId,
        ServiceRequestStatus status = ServiceRequestStatus.Preparing)
    {
        var customerId = TestUsers.Customer.Id;

        var address = AddressFactory.CreateValidAddress(userId: customerId);

        var restaurant = Unwrap(RestaurantFactory.CreateRestaurant(ownerId: TestUsers.Owner1.Id));

        var request = Unwrap(ServiceRequestFactory.CreateServiceRequest(customerId: customerId));

        // Move the request through the allowed transitions until it reaches the wanted status
        switch (status)
        {
            case ServiceRequestStatus.Preparing:
                request.MarkAsPreparing();
                break;
            case ServiceRequestStatus.OutForDelivery:
                request.MarkAsPreparing();
                request.MarkAsOutForDelivery();
                break;
            case ServiceRequestStatus.Delivered:
                request.MarkAsPreparing();
                request.MarkAsOutForDelivery();
                request.MarkAsDelivered();
                break;
            case ServiceRequestStatus.Cancelled:
                request.Cancel("test");
                break;
        }

        // Food order shares the primary key with its service request
        var order = Unwrap(FoodOrderFactory.CreateFoodOrder(
            id: request.Id, restaurantId: restaurant.Id, addressId: address.Id));

        order.AssignDriver(driverId);

        context.TbAddresses.Add(address);
        context.TbRestaurants.Add(restaurant);
        context.TbServiceRequests.Add(request);
        context.TbFoodOrders.Add(order);

        await context.SaveChangesAsync(CancellationToken.None);
        return order.Id;
    }

    private static T Unwrap<T>(UberSy.Domain.Common.Results.Result<T> result)
        => result.IsError
            ? throw new InvalidOperationException($"{result.TopError.Code}: {result.TopError.Description}")
            : result.Value;
}
