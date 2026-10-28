namespace UberSy.Contract.Requests.Orders;

public sealed record PlaceFoodOrderItemRequest(Guid ItemId, int Quantity, List<Guid>? AddOnIds);
