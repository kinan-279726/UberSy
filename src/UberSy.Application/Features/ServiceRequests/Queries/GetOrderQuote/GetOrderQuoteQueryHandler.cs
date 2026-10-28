using MediatR;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.ServiceRequests.Dtos;
using UberSy.Application.Features.ServiceRequests.Services;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Queries.GetOrderQuote;

public sealed class GetOrderQuoteQueryHandler(
    IOwnershipValidator ownership,
    IFoodOrderCalculator calculator)
    : IRequestHandler<GetOrderQuoteQuery, Result<OrderQuoteDto>>
{
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly IFoodOrderCalculator _calculator = calculator;

    public async Task<Result<OrderQuoteDto>> Handle(GetOrderQuoteQuery request, CancellationToken cancellationToken)
    {
        var userResult = await _ownership.IsUser();
        if (userResult.IsError) return userResult.Errors;

        var calculationResult = await _calculator.CalculateAsync(
            userResult.Value, request.RestaurantId, request.AddressId, request.Items, cancellationToken);

        if (calculationResult.IsError) return calculationResult.Errors;

        var calculation = calculationResult.Value;

        return new OrderQuoteDto
        {
            RestaurantId = calculation.Restaurant.Id,
            RestaurantNameAr = calculation.Restaurant.NameAr,
            Lines = calculation.Lines.Select(l => new OrderQuoteLineDto
            {
                ItemId = l.Item.Id,
                ItemNameAr = l.Item.NameAr,
                ItemNameEn = l.Item.NameEn,
                Quantity = l.Quantity,
                UnitPrice = l.Item.Price,
                AddOnNamesAr = l.AddOns.Select(a => a.NameAr).ToList(),
                LineTotal = l.LineTotal,
            }).ToList(),
            ItemsTotal = calculation.ItemsTotal,
            DiscountAmount = calculation.DiscountAmount,
            TaxRate = calculation.Restaurant.TaxRate,
            TaxAmount = calculation.TaxAmount,
            TotalAmount = calculation.TotalAmount,
            DistanceKm = calculation.Delivery.DistanceKm,
            EstimatedDeliveryFee = calculation.Delivery.CustomerFee,
            EstimatedGrandTotal = calculation.TotalAmount + calculation.Delivery.CustomerFee,
        };
    }
}