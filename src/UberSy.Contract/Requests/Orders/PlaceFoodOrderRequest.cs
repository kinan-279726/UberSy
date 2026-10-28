namespace UberSy.Contract.Requests.Orders;

public sealed record PlaceFoodOrderRequest(Guid RestaurantId, Guid AddressId, List<PlaceFoodOrderItemRequest> Items);
