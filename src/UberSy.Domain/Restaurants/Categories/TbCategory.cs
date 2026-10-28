using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Restaurants.Items;
using UberSy.Domain.Restaurants.Printers;

namespace UberSy.Domain.Restaurants.Categories;

public sealed class TbCategory : Entity
{
    public Guid RestaurantId { get; private set; }

    public Guid? PrinterId { get; private set; }

    public string NameAr { get; private set; } = null!;

    public string NameEn { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public byte SortOrder { get; private set; }

    public TbRestaurant Restaurant { get; private set; } = null!;

    public TbPrinter? Printer { get; private set; }

    private readonly List<TbItem> _items = [];
    public IReadOnlyCollection<TbItem> Items => _items.AsReadOnly();

    private TbCategory()
    { }

    private TbCategory(Guid id, Guid restaurantId, Guid? printerId, string nameAr, string nameEn, byte sortOrder)
        : base(id)
    {
        RestaurantId = restaurantId;
        PrinterId = printerId;
        NameAr = nameAr;
        NameEn = nameEn;
        SortOrder = sortOrder;
        IsActive = true;
    }

    public static Result<TbCategory> Create(Guid id, Guid restaurantId, Guid? printerId, string nameAr, string nameEn, byte sortOrder)
    {
        if (restaurantId == Guid.Empty) return CategoryErrors.RestaurantIdRequired;

        var result = ValidateProfile(nameAr, nameEn);
        if (result.IsError) return result.Errors;

        return new TbCategory(id, restaurantId, printerId, nameAr.Trim(), nameEn.Trim(), sortOrder);
    }

    public Result<Updated> UpdateProfile(string nameAr, string nameEn)
    {
        var result = ValidateProfile(nameAr, nameEn);
        if (result.IsError) return result.Errors;

        NameAr = nameAr.Trim();
        NameEn = nameEn.Trim();

        return Result.Updated;
    }

    public Result<Updated> UpdateSortOrder(byte sortOrder)
    {
        SortOrder = sortOrder;
        return Result.Updated;
    }

    public Result<Updated> AssignPrinter(Guid printerId)
    {
        PrinterId = printerId;
        return Result.Updated;
    }

    public Result<Updated> RemovePrinter()
    {
        PrinterId = null;
        return Result.Updated;
    }

    public Result<Updated> Activate()
    {
        if (IsActive) return CategoryErrors.CategoryAlreadyActive;

        IsActive = true;

        return Result.Updated;
    }

    public Result<Updated> Deactivate()
    {
        if (!IsActive) return CategoryErrors.CategoryAlreadyInactive;

        IsActive = false;

        return Result.Updated;
    }

    private static Result<Success> ValidateProfile(string nameAr, string nameEn)
    {
        if (string.IsNullOrWhiteSpace(nameAr)) return CategoryErrors.NameArRequired;
        if (nameAr.Length < 2) return CategoryErrors.NameArTooShort;
        if (nameAr.Length > 100) return CategoryErrors.NameArTooLong;

        if (string.IsNullOrWhiteSpace(nameEn)) return CategoryErrors.NameEnRequired;
        if (nameEn.Length < 2) return CategoryErrors.NameEnTooShort;
        if (nameEn.Length > 100) return CategoryErrors.NameEnTooLong;

        return Result.Success;
    }
}