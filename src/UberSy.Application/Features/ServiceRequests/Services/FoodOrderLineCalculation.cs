using UberSy.Domain.Restaurants.AddOns;
using UberSy.Domain.Restaurants.Items;

namespace UberSy.Application.Features.ServiceRequests.Services;

public sealed class FoodOrderLineCalculation
{
    public TbItem Item { get; init; } = null!;
    public int Quantity { get; init; }
    public List<TbAddOn> AddOns { get; init; } = [];
    public decimal LineTotal { get; init; }
}
