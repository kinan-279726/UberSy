using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Restaurants.Items;
using UberSy.Domain.ServiceRequests.FoodOrders;

namespace UberSy.Domain.Restaurants.AddOns;

public sealed class TbAddOn : Entity
{
    public Guid RestaurantId { get; private set; }
 
    public string NameAr { get; private set; } = null!;
    
    public string NameEn { get; private set; } = null!;
    
    public decimal Price { get; private set; }
    
    public bool IsActive { get; private set; }

    public TbRestaurant Restaurant { get; private set; } = null!;

    private readonly List<TbOrderItemAddOn> _orderItemAddOns = [];
    public IReadOnlyCollection<TbOrderItemAddOn> OrderItemAddOns => _orderItemAddOns.AsReadOnly();

    private readonly List<TbItem> _items = [];
    public IReadOnlyCollection<TbItem> Items => _items.AsReadOnly();

    private TbAddOn()
    { }

    private TbAddOn(Guid id, Guid restaurantId, string nameAr, string nameEn, decimal price)
        : base(id)
    {
        RestaurantId = restaurantId;
        NameAr = nameAr;
        NameEn = nameEn;
        Price = price;
        IsActive = true;
    }

    public static Result<TbAddOn> Create(Guid id, Guid restaurantId, string nameAr, string nameEn, decimal price)
    {
        if (restaurantId == Guid.Empty) return AddOnErrors.RestaurantIdRequired;

        var profile = ValidateProfile(nameAr, nameEn);
        if (profile.IsError) return profile.Errors;

        var priceResult = ValidatePrice(price);
        if (priceResult.IsError) return priceResult.Errors;

        return new TbAddOn(id, restaurantId, nameAr.Trim(), nameEn.Trim(), price);
    }

    public Result<Updated> Update(string nameAr, string nameEn ,decimal price)
    {
        var result = ValidateProfile(nameAr, nameEn);
        if (result.IsError) return result.Errors;

        result = ValidatePrice(price);
        if (result.IsError) return result.Errors;

        NameAr = nameAr.Trim();
        NameEn = nameEn.Trim();
        Price = price;

        return Result.Updated;
    }
    public Result<Updated> Activate()
    {
        if (IsActive) return AddOnErrors.AddOnAlreadyActive;

        IsActive = true;

        return Result.Updated;
    }

    public Result<Updated> Deactivate()
    {
        if (!IsActive) return AddOnErrors.AddOnAlreadyInactive;

        IsActive = false;

        return Result.Updated;
    }

    private static Result<Success> ValidateProfile(string nameAr, string nameEn)
    {
        if (string.IsNullOrWhiteSpace(nameAr)) return AddOnErrors.NameArRequired;
        if (nameAr.Length < 2) return AddOnErrors.NameArTooShort;
        if (nameAr.Length > 100) return AddOnErrors.NameArTooLong;

        if (string.IsNullOrWhiteSpace(nameEn)) return AddOnErrors.NameEnRequired;
        if (nameEn.Length < 2) return AddOnErrors.NameEnTooShort;
        if (nameEn.Length > 100) return AddOnErrors.NameEnTooLong;

        return Result.Success;
    }

    private static Result<Success> ValidatePrice(decimal price)
    {
        if (price <= 0) return AddOnErrors.PriceZero;

        return Result.Success;
    }
}
