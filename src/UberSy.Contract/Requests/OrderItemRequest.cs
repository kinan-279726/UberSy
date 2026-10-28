namespace UberSy.Contract.Requests;
public sealed record OrderItemRequest(Guid ItemId, int Quantity, List<Guid> AddOnIds);
