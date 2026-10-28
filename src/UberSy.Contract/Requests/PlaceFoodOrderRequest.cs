namespace UberSy.Contract.Requests;

public sealed record PlaceFoodOrderRequest(Guid RestaurantId, Guid AddressId, List<OrderItemRequest> Items);