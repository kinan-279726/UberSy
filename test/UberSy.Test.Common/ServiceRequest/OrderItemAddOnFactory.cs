using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.FoodOrders;

namespace UberSy.Test.Common.ServiceRequest;

public static class OrderItemAddOnFactory
{
    public static Result<TbOrderItemAddOn> CreateOrderItemAddOn(
        Guid? id = null,
        Guid? orderItemId = null,
        Guid? addOnId = null,
        decimal? price = null)
    {
        return TbOrderItemAddOn.Create(
            id ?? Guid.NewGuid(),
            orderItemId ?? Guid.NewGuid(),
            addOnId ?? Guid.NewGuid(),
            price ?? 2.5m);
    }
}
