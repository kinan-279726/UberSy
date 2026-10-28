using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Restaurants.Items;

namespace UberSy.Domain.ServiceRequests.FoodOrders;

public sealed class TbOrderItem : Entity
{
    public Guid FoodOrderId { get; private set; }

    public Guid ItemId { get; private set; }
    
    public int Quantity { get; private set; }
    
    public decimal UnitPrice { get; private set; }
    
    public string? Note { get; private set; }

    public TbFoodOrder FoodOrder { get; private set; } = null!;
    public TbItem Item { get; private set; } = null!;

    private readonly List<TbOrderItemAddOn> _addOns = [];
    public IReadOnlyCollection<TbOrderItemAddOn> AddOns => _addOns.AsReadOnly();

    private TbOrderItem()
    { }

    private TbOrderItem(Guid id, Guid foodOrderId, Guid itemId, int quantity, decimal unitPrice, string? note)
        : base(id)
    {
        FoodOrderId = foodOrderId;
        ItemId = itemId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Note = note;
    }

    public static Result<TbOrderItem> Create(Guid id, Guid foodOrderId, Guid itemId, int quantity, decimal unitPrice, string? note)
    {
        if (itemId == Guid.Empty) return OrderItemErrors.ItemIdRequired;
        if (quantity <= 0) return OrderItemErrors.QuantityZero;
        if (unitPrice <= 0) return OrderItemErrors.UnitPriceZero;
        if (note is { Length: > 100 }) return OrderItemErrors.NoteTooLong;

        return new TbOrderItem(id, foodOrderId, itemId, quantity, unitPrice, note?.Trim());
    }

    public Result<Updated> UpdateQuantity(int quantity)
    {
        if (quantity <= 0) return OrderItemErrors.QuantityZero;

        Quantity = quantity;

        return Result.Updated;
    }

    public Result<Updated> UpdateNote(string? note)
    {
        if (note is { Length: > 100 }) return OrderItemErrors.NoteTooLong;

        Note = note?.Trim();

        return Result.Updated;
    }
}