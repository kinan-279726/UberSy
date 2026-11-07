using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.FoodOrders;

namespace UberSy.Test.Common.ServiceRequest;

public static class FoodOrderFactory
{
    public static Result<TbFoodOrder> CreateFoodOrder(
        Guid? id = null,
        Guid? restaurantId = null,
        Guid? addressId = null,
        DateTimeOffset? responseTimeout = null)
    {
        return TbFoodOrder.Create(
            id ?? Guid.NewGuid(),
            restaurantId ?? Guid.NewGuid(),
            addressId ?? Guid.NewGuid(),
            responseTimeout ?? DateTimeOffset.UtcNow.AddMinutes(5));
    }
}
