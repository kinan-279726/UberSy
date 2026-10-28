using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Restaurants;

namespace UberSy.Application.Features.ServiceRequests.Services;

public sealed class FoodOrderCalculation
{
    public TbRestaurant Restaurant { get; init; } = null!;
    public List<FoodOrderLineCalculation> Lines { get; init; } = [];
    public decimal ItemsTotal { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalAmount { get; init; }
    public DeliveryQuote Delivery { get; init; } = null!;
}