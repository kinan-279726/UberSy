using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Restaurants.AddOns;

namespace UberSy.Domain.ServiceRequests.FoodOrders;

public sealed class TbOrderItemAddOn : Entity
{
    public Guid OrderItemId { get; private set; }
    public Guid AddOnId { get; private set; }
    public decimal Price { get; private set; }

    public TbAddOn AddOn { get; private set; } = null!;
    public TbOrderItem OrderItem { get; private set; } = null!;

    private TbOrderItemAddOn()
    { }

    private TbOrderItemAddOn(Guid id, Guid orderItemId, Guid addOnId, decimal price)
        : base(id)
    {
        OrderItemId = orderItemId;
        AddOnId = addOnId;
        Price = price;
    }

    public static Result<TbOrderItemAddOn> Create(Guid id, Guid orderItemId, Guid addOnId, decimal price)
    {
        if (addOnId == Guid.Empty) return OrderItemErrors.ItemIdRequired;
        if (price < 0) return OrderItemErrors.AddOnPriceNegative;

        return new TbOrderItemAddOn(id, orderItemId, addOnId, price);
    }
}