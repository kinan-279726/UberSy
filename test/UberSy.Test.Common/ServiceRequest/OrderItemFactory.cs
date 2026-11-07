using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.FoodOrders;

namespace UberSy.Test.Common.ServiceRequest;

public static class OrderItemFactory
{
    public static Result<TbOrderItem> CreateOrderItem(
        Guid? id = null,
        Guid? foodOrderId = null,
        Guid? itemId = null,
        int? quantity = null,
        decimal? unitPrice = null,
        string? note = null)
    {
        return TbOrderItem.Create(
            id ?? Guid.NewGuid(),
            foodOrderId ?? Guid.NewGuid(),
            itemId ?? Guid.NewGuid(),
            quantity ?? 2,
            unitPrice ?? 25m,
            note);
    }
}
