using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Discounts;
using UberSy.Domain.Restaurants.AddOns;
using UberSy.Domain.Restaurants.Categories;
using UberSy.Domain.ServiceRequests.FoodOrders;

namespace UberSy.Domain.Restaurants.Items;

public sealed class TbItem : Entity
{
    public Guid RestaurantId { get; private set; }
    
    public Guid CategoryId { get; private set; }
    
    public string NameAr { get; private set; } = null!;
    
    public string NameEn { get; private set; } = null!;
    
    public string DescriptionAr { get; private set; } = null!;
    
    public string DescriptionEn { get; private set; } = null!;
    
    public decimal Price { get; private set; }
    
    public string ImageUrl { get; private set; } = null!;
    
    public int? Calories { get; private set; }
    
    public bool IsActive { get; private set; }
    
    public byte SortOrder { get; private set; }

    public TbRestaurant Restaurant { get; private set; } = null!;
   
    public TbCategory Category { get; private set; } = null!;

    private readonly List<TbOrderItem> _orderItems = [];
    public IReadOnlyCollection<TbOrderItem> OrderItems => _orderItems.AsReadOnly();

    private readonly List<TbAddOn> _addOns = [];
    public IReadOnlyCollection<TbAddOn> AddOns => _addOns.AsReadOnly();

    private readonly List<TbDiscount> _discounts = [];
    public IReadOnlyCollection<TbDiscount> Discounts => _discounts.AsReadOnly();

    private TbItem()
    { }

    private TbItem(Guid id, Guid restaurantId, Guid categoryId, string nameAr, string nameEn,
        string descriptionAr, string descriptionEn, decimal price, string imageUrl, int? calories, byte sortOrder)
        : base(id)
    {
        RestaurantId = restaurantId;
        CategoryId = categoryId;
        NameAr = nameAr;
        NameEn = nameEn;
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;
        Price = price;
        ImageUrl = imageUrl;
        Calories = calories;
        SortOrder = sortOrder;
        IsActive = true;
    }

    public static Result<TbItem> Create(Guid id, Guid restaurantId, Guid categoryId, string nameAr, string nameEn,
        string descriptionAr, string descriptionEn, decimal price, string imageUrl, int? calories, byte sortOrder)
    {
        if (restaurantId == Guid.Empty) return ItemErrors.RestaurantIdRequired;
        if (categoryId == Guid.Empty) return ItemErrors.CategoryIdRequired;

        var profile = ValidateProfile(nameAr, nameEn, descriptionAr, descriptionEn, imageUrl);
        if (profile.IsError) return profile.Errors;

        var priceResult = ValidatePrice(price);
        if (priceResult.IsError) return priceResult.Errors;

        var caloriesResult = ValidateCalories(calories);
        if (caloriesResult.IsError) return caloriesResult.Errors;

        return new TbItem(id, restaurantId, categoryId, nameAr.Trim(), nameEn.Trim(), descriptionAr.Trim(),
            descriptionEn.Trim(), price, imageUrl.Trim(), calories, sortOrder);
    }

    public Result<Updated> UpdateProfile(string nameAr, string nameEn, string descriptionAr, string descriptionEn, string imageUrl)
    {
        var result = ValidateProfile(nameAr, nameEn, descriptionAr, descriptionEn, imageUrl);
        if (result.IsError) return result.Errors;

        NameAr = nameAr.Trim();
        NameEn = nameEn.Trim();
        DescriptionAr = descriptionAr.Trim();
        DescriptionEn = descriptionEn.Trim();
        ImageUrl = imageUrl.Trim();

        return Result.Updated;
    }

    public Result<Updated> UpdatePrice(decimal price)
    {
        var result = ValidatePrice(price);
        if (result.IsError) return result.Errors;

        Price = price;

        return Result.Updated;
    }

    public Result<Updated> UpdateCalories(int? calories)
    {
        var result = ValidateCalories(calories);
        if (result.IsError) return result.Errors;

        Calories = calories;

        return Result.Updated;
    }

    public Result<Updated> UpdateSortOrder(byte sortOrder)
    {
        SortOrder = sortOrder;
        return Result.Updated;
    }

    public Result<Updated> MoveToCategory(Guid categoryId)
    {
        if (categoryId == Guid.Empty) return ItemErrors.CategoryIdRequired;

        CategoryId = categoryId;

        return Result.Updated;
    }

    public Result<Updated> Activate()
    {
        if (IsActive) return ItemErrors.ItemAlreadyActive;

        IsActive = true;

        return Result.Updated;
    }

    public Result<Updated> Deactivate()
    {
        if (!IsActive) return ItemErrors.ItemAlreadyInactive;

        IsActive = false;

        return Result.Updated;
    }

    public Result<Updated> AssignAddOn(TbAddOn addOn)
    {
        if (addOn.RestaurantId != RestaurantId) return ItemErrors.AddOnFromDifferentRestaurant;
        if (_addOns.Any(a => a.Id == addOn.Id)) return ItemErrors.AddOnAlreadyAssigned;

        _addOns.Add(addOn);
        return Result.Updated;
    }

    public Result<Updated> RemoveAddOn(Guid addOnId)
    {
        var addOn = _addOns.FirstOrDefault(a => a.Id == addOnId);
        if (addOn is null) return ItemErrors.AddOnNotAssigned;

        _addOns.Remove(addOn);
        return Result.Updated;
    }

    private static Result<Success> ValidateProfile(string nameAr, string nameEn, string descriptionAr, string descriptionEn, string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(nameAr)) return ItemErrors.NameArRequired;
        if (nameAr.Length < 2) return ItemErrors.NameArTooShort;
        if (nameAr.Length > 100) return ItemErrors.NameArTooLong;

        if (string.IsNullOrWhiteSpace(nameEn)) return ItemErrors.NameEnRequired;
        if (nameEn.Length < 2) return ItemErrors.NameEnTooShort;
        if (nameEn.Length > 100) return ItemErrors.NameEnTooLong;

        if (string.IsNullOrWhiteSpace(descriptionAr)) return ItemErrors.DescriptionArRequired;
        if (descriptionAr.Length > 1000) return ItemErrors.DescriptionArTooLong;

        if (string.IsNullOrWhiteSpace(descriptionEn)) return ItemErrors.DescriptionEnRequired;
        if (descriptionEn.Length > 1000) return ItemErrors.DescriptionEnTooLong;

        if (string.IsNullOrWhiteSpace(imageUrl)) return ItemErrors.ImageUrlRequired;
        if (imageUrl.Length > 1000) return ItemErrors.ImageUrlTooLong;

        return Result.Success;
    }

    private static Result<Success> ValidatePrice(decimal price)
    {
        if (price <= 0) return ItemErrors.PriceZero;

        return Result.Success;
    }

    private static Result<Success> ValidateCalories(int? calories)
    {
        if (calories is < 0) return ItemErrors.CaloriesNegative;

        return Result.Success;
    }
}