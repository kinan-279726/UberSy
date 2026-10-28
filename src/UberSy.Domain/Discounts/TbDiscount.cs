using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Discounts.Enums;
using UberSy.Domain.Restaurants;
using UberSy.Domain.Restaurants.Items;
using UberSy.Domain.ServiceRequests.FoodOrders;

namespace UberSy.Domain.Discounts;

public sealed class TbDiscount : AuditableEntity
{
    public Guid? RestaurantId { get; private set; }
    public string? Code { get; private set; }
    public DiscountType Type { get; private set; }
    public decimal Value { get; private set; }
    public DiscountScope Scope { get; private set; }
    public decimal? MinOrderAmount { get; private set; }
    public int? MaxUsageCount { get; private set; }
    public byte? MaxUsagePerCustomer { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public bool IsActive { get; private set; }

    // مرجعية فقط
    public TbRestaurant? Restaurant { get; private set; }

    private readonly List<TbDiscountUsage> _usages = [];
    public IReadOnlyCollection<TbDiscountUsage> Usages => _usages.AsReadOnly();

    private readonly List<TbFoodOrder> _foodOrders = [];
    public IReadOnlyCollection<TbFoodOrder> FoodOrders => _foodOrders.AsReadOnly();

    private readonly List<TbItem> _items = [];
    public IReadOnlyCollection<TbItem> Items => _items.AsReadOnly();

    private TbDiscount()
    { }

    private TbDiscount(Guid id, Guid? restaurantId, string? code, DiscountType type, decimal value, DiscountScope scope,
        decimal? minOrderAmount, int? maxUsageCount, byte? maxUsagePerCustomer, DateTime startDate, DateTime endDate)
        : base(id)
    {
        RestaurantId = restaurantId;
        Code = code;
        Type = type;
        Value = value;
        Scope = scope;
        MinOrderAmount = minOrderAmount;
        MaxUsageCount = maxUsageCount;
        MaxUsagePerCustomer = maxUsagePerCustomer;
        StartDate = startDate;
        EndDate = endDate;
        IsActive = true;
    }

    public static Result<TbDiscount> Create(Guid id, Guid? restaurantId, string? code, DiscountType type, decimal value,
        DiscountScope scope, decimal? minOrderAmount, int? maxUsageCount, byte? maxUsagePerCustomer, DateTime startDate, DateTime endDate)
    {
        var result = Validate(code, type, value, minOrderAmount, maxUsageCount, startDate, endDate);
        if (result.IsError) return result.Errors;

        return new TbDiscount(id, restaurantId, code?.Trim(), type, value, scope, minOrderAmount, maxUsageCount,
            maxUsagePerCustomer, startDate, endDate);
    }

    public Result<Updated> UpdateValue(decimal value)
    {
        if (value <= 0) return DiscountErrors.ValueZero;
        if (Type == DiscountType.Percentage && value > 100) return DiscountErrors.PercentageExceeds100;

        Value = value;

        return Result.Updated;
    }

    public Result<Updated> UpdateSchedule(DateTime startDate, DateTime endDate)
    {
        if (endDate <= startDate) return DiscountErrors.EndDateBeforeStartDate;

        StartDate = startDate;
        EndDate = endDate;

        return Result.Updated;
    }

    public Result<Updated> Activate()
    {
        if (IsActive) return DiscountErrors.DiscountAlreadyActive;

        IsActive = true;

        return Result.Updated;
    }

    public Result<Updated> Deactivate()
    {
        if (!IsActive) return DiscountErrors.DiscountAlreadyInactive;

        IsActive = false;

        return Result.Updated;
    }

    private static Result<Success> Validate(string? code, DiscountType type, decimal value, decimal? minOrderAmount,
        int? maxUsageCount, DateTime startDate, DateTime endDate)
    {
        if (code is { Length: > 20 }) return DiscountErrors.CodeTooLong;
        if (!Enum.IsDefined(type)) return DiscountErrors.ValueZero;

        if (value <= 0) return DiscountErrors.ValueZero;
        if (type == DiscountType.Percentage && value > 100) return DiscountErrors.PercentageExceeds100;

        if (minOrderAmount is < 0) return DiscountErrors.MinOrderAmountNegative;
        if (maxUsageCount is <= 0) return DiscountErrors.MaxUsageCountZero;
        if (endDate <= startDate) return DiscountErrors.EndDateBeforeStartDate;

        return Result.Success;
    }
}