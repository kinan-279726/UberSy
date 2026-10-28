using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Application.Features.ServiceRequests.Services;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Restaurants.AddOns;
using UberSy.Domain.Restaurants.Items;
using UberSy.Domain.ServiceRequests;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Domain.ServiceRequests.FoodOrders;

namespace UberSy.Application.Features.ServiceRequests.Commands.PlaceFoodOrder;

public sealed class PlaceFoodOrderCommandHandler(
    IOwnershipValidator ownership,
    IApplicationDbContext context,
    IFoodOrderCalculator calculator,
    IConfiguration configuration,
    ILogger<PlaceFoodOrderCommandHandler> logger)
    : IRequestHandler<PlaceFoodOrderCommand, Result<FoodOrderSummaryDto>>
{
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly IApplicationDbContext _context = context;
    private readonly IFoodOrderCalculator _calculator = calculator;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<PlaceFoodOrderCommandHandler> _logger = logger;

    public async Task<Result<FoodOrderSummaryDto>> Handle(
        PlaceFoodOrderCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _ownership.IsUser();
        if (userResult.IsError) return userResult.Errors;

        var userId = userResult.Value;

        var calculationResult = await _calculator.CalculateAsync(
            userId, request.RestaurantId, request.AddressId, request.Items, cancellationToken);

        if (calculationResult.IsError) return calculationResult.Errors;

        var calculation = calculationResult.Value;

        // إنشاء ServiceRequest + FoodOrder بنفس الـ Id (Class Table Inheritance)
        var serviceRequestId = Guid.NewGuid();

        var serviceRequestResult = TbServiceRequest.Create(
            id: serviceRequestId,
            customerId: userId,
            serviceType: ServiceType.Food,
            totalAmount: calculation.TotalAmount);

        if (serviceRequestResult.IsError)
        {
            _logger.LogError("Failed To Create ServiceRequest: {Error}", serviceRequestResult.TopError.Description);
            return serviceRequestResult.Errors;
        }

        var responseTimeoutMinutes = _configuration.GetValue<int>("OrderSettings:RestaurantResponseTimeoutMinutes");

        var foodOrderResult = TbFoodOrder.Create(
            id: serviceRequestId,
            restaurantId: request.RestaurantId,
            addressId: request.AddressId,
            responseTimeout: DateTimeOffset.UtcNow.AddMinutes(responseTimeoutMinutes));

        if (foodOrderResult.IsError)
        {
            _logger.LogError("Failed To Create FoodOrder: {Error}", foodOrderResult.TopError.Description);
            return foodOrderResult.Errors;
        }

        var serviceRequest = serviceRequestResult.Value;
        var foodOrder = foodOrderResult.Value;

        _context.TbServiceRequests.Add(serviceRequest);
        _context.TbFoodOrders.Add(foodOrder);

        // Snapshot للأسعار
        foreach (var line in calculation.Lines)
        {
            var orderItemResult = TbOrderItem.Create(
                id: Guid.NewGuid(),
                foodOrderId: foodOrder.Id,
                itemId: line.Item.Id,
                quantity: line.Quantity,
                unitPrice: line.Item.Price,
                note: null);

            if (orderItemResult.IsError)
            {
                _logger.LogError("Failed To Create OrderItem: {Error}", orderItemResult.TopError.Description);
                return orderItemResult.Errors;
            }

            var orderItem = orderItemResult.Value;
            _context.TbOrderItems.Add(orderItem);

            foreach (var addOn in line.AddOns)
            {
                var orderItemAddOnResult = TbOrderItemAddOn.Create(
                    id: Guid.NewGuid(),
                    orderItemId: orderItem.Id,
                    addOnId: addOn.Id,
                    price: addOn.Price);

                if (orderItemAddOnResult.IsError)
                {
                    _logger.LogError("Failed To Create OrderItemAddOn: {Error}", orderItemAddOnResult.TopError.Description);
                    return orderItemAddOnResult.Errors;
                }

                _context.TbOrderItemAddOns.Add(orderItemAddOnResult.Value);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "FoodOrder '{ServiceRequestId}' Placed By Customer '{CustomerId}' For Restaurant '{RestaurantId}', Total: {Total}.",
            serviceRequest.Id, userId, request.RestaurantId, calculation.TotalAmount);

        return new FoodOrderSummaryDto
        {
            ServiceRequestId = serviceRequest.Id,
            TrackingNumber = serviceRequest.TrackingNumber,
            RestaurantId = request.RestaurantId,
            Status = (byte)serviceRequest.Status,
            ItemsTotal = calculation.ItemsTotal,
            DiscountAmount = calculation.DiscountAmount,
            TaxAmount = calculation.TaxAmount,
            TotalAmount = calculation.TotalAmount,
            DistanceKm = calculation.Delivery.DistanceKm,
            EstimatedDeliveryFee = calculation.Delivery.CustomerFee,
            EstimatedGrandTotal = calculation.TotalAmount + calculation.Delivery.CustomerFee,
            ResponseDeadlineUtc = foodOrder.ResponseTimeout,
        };
    }
}