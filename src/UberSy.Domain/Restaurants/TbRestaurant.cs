using NetTopologySuite.Geometries;
using System.Text.RegularExpressions;
using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Discounts;
using UberSy.Domain.Reservations;
using UberSy.Domain.Restaurants.AddOns;
using UberSy.Domain.Restaurants.Categories;
using UberSy.Domain.Restaurants.Images;
using UberSy.Domain.Restaurants.Items;
using UberSy.Domain.Restaurants.Printers;
using UberSy.Domain.Reviews;
using UberSy.Domain.ServiceRequests.FoodOrders;
using UberSy.Domain.Tags;

namespace UberSy.Domain.Restaurants;

public sealed class TbRestaurant : AuditableEntity
{
    public Guid OwnerId { get; private set; }

    public string NameAr { get; private set; } = null!;

    public string NameEn { get; private set; } = null!;
    
    public string? DescriptionAr { get; private set; }
    
    public string? DescriptionEn { get; private set; }
    
    public string Phone { get; private set; } = null!;
    
    public string? Email { get; private set; }
    
    public string? LogoUrl { get; private set; }
    
    public string? CoverUrl { get; private set; }
    
    public decimal CommissionRate { get; private set; }
    
    public decimal TaxRate { get; private set; }
    
    public TimeOnly OpeningTime { get; private set; }
    
    public TimeOnly ClosingTime { get; private set; }
    
    public decimal MinOrderAmount { get; private set; }
    
    public decimal MaxOrderAmount { get; private set; }
    
    public bool IsOpenNow { get; private set; }
    
    public Point Boundary { get; private set; } = null!;
    
    public decimal Rating { get; private set; }
    
    public bool IsApproved { get; private set; }
    
    public bool AcceptReservation { get; private set; }

    private readonly List<TbCategory> _categories = [];
    public IReadOnlyCollection<TbCategory> Categories => _categories.AsReadOnly();

    private readonly List<TbItem> _items = [];
    public IReadOnlyCollection<TbItem> Items => _items.AsReadOnly();

    private readonly List<TbAddOn> _addOns = [];
    public IReadOnlyCollection<TbAddOn> AddOns => _addOns.AsReadOnly();

    private readonly List<TbPrinter> _printers = [];
    public IReadOnlyCollection<TbPrinter> Printers => _printers.AsReadOnly();

    private readonly List<TbRestaurantImage> _images = [];
    public IReadOnlyCollection<TbRestaurantImage> Images => _images.AsReadOnly();

    private readonly List<TbDiscount> _discounts = [];
    public IReadOnlyCollection<TbDiscount> Discounts => _discounts.AsReadOnly();

    private readonly List<TbFoodOrder> _foodOrders = [];
    public IReadOnlyCollection<TbFoodOrder> FoodOrders => _foodOrders.AsReadOnly();

    private readonly List<TbReview> _reviews = [];
    public IReadOnlyCollection<TbReview> Reviews => _reviews.AsReadOnly();

    private readonly List<TbTableReservation> _tableReservations = [];
    public IReadOnlyCollection<TbTableReservation> TableReservations => _tableReservations.AsReadOnly();

    private readonly List<TbTag> _tags = [];
    public IReadOnlyCollection<TbTag> Tags => _tags.AsReadOnly();

    private TbRestaurant()
    { }

    private TbRestaurant(
        Guid id, Guid ownerId, string nameAr, string nameEn, string? descriptionAr, string? descriptionEn,
        string phone, string? email, decimal commissionRate, decimal taxRate,
        TimeOnly openingTime, TimeOnly closingTime, decimal minOrderAmount, decimal maxOrderAmount, Point boundary)
        : base(id)
    {
        OwnerId = ownerId;
        NameAr = nameAr;
        NameEn = nameEn;
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;
        Phone = phone;
        Email = email;
        CommissionRate = commissionRate;
        TaxRate = taxRate;
        OpeningTime = openingTime;
        ClosingTime = closingTime;
        MinOrderAmount = minOrderAmount;
        MaxOrderAmount = maxOrderAmount;
        Boundary = boundary;
        IsOpenNow = false;
        IsApproved = false;
        AcceptReservation = false;
        Rating = 0;
    }

    public static Result<TbRestaurant> Create(
        Guid id, Guid ownerId, string nameAr, string nameEn, string? descriptionAr, string? descriptionEn,
        string phone, string? email, decimal commissionRate, decimal taxRate,
        TimeOnly openingTime, TimeOnly closingTime, decimal minOrderAmount, decimal maxOrderAmount, Point boundary)
    {
        if (ownerId == Guid.Empty)
            return RestaurantErrors.OwnerIdRequired;

        var profile = ValidateProfile(nameAr, nameEn, descriptionAr, descriptionEn, phone, email);
        if (profile.IsError) return profile.Errors;

        var rates = ValidateRates(commissionRate, taxRate);
        if (rates.IsError) return rates.Errors;

        var hours = ValidateHours(openingTime, closingTime);
        if (hours.IsError) return hours.Errors;

        var limits = ValidateOrderLimits(minOrderAmount, maxOrderAmount);
        if (limits.IsError) return limits.Errors;

        var boundaryResult = ValidateBoundary(boundary);
        if (boundaryResult.IsError) return boundaryResult.Errors;

        return new TbRestaurant(id, ownerId, nameAr.Trim(), nameEn.Trim(), descriptionAr?.Trim(), descriptionEn?.Trim(),
            phone.Trim(), email?.Trim(), commissionRate, taxRate, openingTime, closingTime,
            minOrderAmount, maxOrderAmount, boundary);
    }

    public Result<Updated> UpdateProfile(string nameAr, string nameEn, string? descriptionAr, string? descriptionEn, string phone, string? email)
    {
        var result = ValidateProfile(nameAr, nameEn, descriptionAr, descriptionEn, phone, email);
        if (result.IsError) return result.Errors;

        NameAr = nameAr.Trim();
        NameEn = nameEn.Trim();
        DescriptionAr = descriptionAr?.Trim();
        DescriptionEn = descriptionEn?.Trim();
        Phone = phone.Trim();
        Email = email?.Trim();

        return Result.Updated;
    }

    public Result<Updated> UpdateImages(string? logoUrl, string? coverUrl)
    {
        if (logoUrl is { Length: > 1000 }) return RestaurantErrors.LogoUrlTooLong;
        if (coverUrl is { Length: > 1000 }) return RestaurantErrors.CoverUrlTooLong;

        LogoUrl = logoUrl;
        CoverUrl = coverUrl;

        return Result.Updated;
    }

    public Result<Updated> UpdateCommissionRate(decimal commissionRate)
    {
        if (commissionRate < 0) return RestaurantErrors.CommissionRateNegative;
        if (commissionRate > 100) return RestaurantErrors.CommissionRateExceedsMaximum;

        CommissionRate = commissionRate;

        return Result.Updated;
    }

    public Result<Updated> UpdateTaxRate(decimal taxRate)
    {
        if (taxRate < 0) return RestaurantErrors.TaxRateNegative;
        if (taxRate > 100) return RestaurantErrors.TaxRateExceedsMaximum;

        TaxRate = taxRate;

        return Result.Updated;
    }

    public Result<Updated> UpdateOperatingHours(TimeOnly openingTime, TimeOnly closingTime)
    {
        var result = ValidateHours(openingTime, closingTime);
        if (result.IsError) return result.Errors;

        OpeningTime = openingTime;
        ClosingTime = closingTime;

        return Result.Updated;
    }

    public Result<Updated> UpdateOrderLimits(decimal minOrderAmount, decimal maxOrderAmount)
    {
        var result = ValidateOrderLimits(minOrderAmount, maxOrderAmount);
        if (result.IsError) return result.Errors;

        MinOrderAmount = minOrderAmount;
        MaxOrderAmount = maxOrderAmount;

        return Result.Updated;
    }

    public Result<Updated> UpdateBoundary(Point boundary)
    {
        var result = ValidateBoundary(boundary);
        if (result.IsError) return result.Errors;

        Boundary = boundary;

        return Result.Updated;
    }

    public Result<Updated> Approve()
    {
        if (IsApproved) return RestaurantErrors.RestaurantAlreadyApproved;

        IsApproved = true;

        return Result.Updated;
    }

    public Result<Updated> Reject()
    {
        if (!IsApproved) return RestaurantErrors.RestaurantAlreadyRejected;

        IsApproved = false;

        return Result.Updated;
    }

    public Result<Updated> Open()
    {
        if (!IsApproved) return RestaurantErrors.RestaurantNotApproved;
        if (IsOpenNow) return RestaurantErrors.RestaurantAlreadyOpen;

        IsOpenNow = true;

        return Result.Updated;
    }

    public Result<Updated> Close()
    {
        if (!IsOpenNow) return RestaurantErrors.RestaurantAlreadyClosed;

        IsOpenNow = false;

        return Result.Updated;
    }

    public Result<Updated> EnableReservation()
    {
        AcceptReservation = true;
        return Result.Updated;
    }

    public Result<Updated> DisableReservation()
    {
        AcceptReservation = false;
        return Result.Updated;
    }

    public Result<Updated> UpdateRating(decimal rating)
    {
        if (rating < 0) return RestaurantErrors.RatingNegative;
        if (rating > 5) return RestaurantErrors.RatingExceedsMaximum;

        Rating = rating;

        return Result.Updated;
    }

    private static Result<Success> ValidateProfile(string nameAr, string nameEn, string? descriptionAr, string? descriptionEn, string phone, string? email)
    {
        if (string.IsNullOrWhiteSpace(nameAr)) return RestaurantErrors.NameArRequired;
        if (nameAr.Length < 2) return RestaurantErrors.NameArTooShort;
        if (nameAr.Length > 100) return RestaurantErrors.NameArTooLong;

        if (string.IsNullOrWhiteSpace(nameEn)) return RestaurantErrors.NameEnRequired;
        if (nameEn.Length < 2) return RestaurantErrors.NameEnTooShort;
        if (nameEn.Length > 100) return RestaurantErrors.NameEnTooLong;

        if (descriptionAr is { Length: > 1000 }) return RestaurantErrors.DescriptionArTooLong;
        if (descriptionEn is { Length: > 1000 }) return RestaurantErrors.DescriptionEnTooLong;

        if (string.IsNullOrWhiteSpace(phone)) return RestaurantErrors.PhoneRequired;
        if (!Regex.IsMatch(phone, @"^\+9639\d{8}$")) return RestaurantErrors.PhoneInvalid;
        if (phone.Length > 20) return RestaurantErrors.PhoneTooLong;

        if (email is { Length: > 100 }) return RestaurantErrors.EmailTooLong;

        return Result.Success;
    }

    private static Result<Success> ValidateRates(decimal commissionRate, decimal taxRate)
    {
        if (commissionRate < 0) return RestaurantErrors.CommissionRateNegative;
        if (commissionRate > 100) return RestaurantErrors.CommissionRateExceedsMaximum;

        if (taxRate < 0) return RestaurantErrors.TaxRateNegative;
        if (taxRate > 100) return RestaurantErrors.TaxRateExceedsMaximum;

        return Result.Success;
    }

    private static Result<Success> ValidateHours(TimeOnly openingTime, TimeOnly closingTime)
    {
        if (closingTime <= openingTime) return RestaurantErrors.ClosingTimeMustBeAfterOpeningTime;

        return Result.Success;
    }

    private static Result<Success> ValidateOrderLimits(decimal minOrderAmount, decimal maxOrderAmount)
    {
        if (minOrderAmount < 0) return RestaurantErrors.MinOrderAmountNegative;
        if (maxOrderAmount < 0) return RestaurantErrors.MaxOrderAmountNegative;
        if (maxOrderAmount <= minOrderAmount) return RestaurantErrors.MaxOrderAmountMustBeGreaterThanMin;

        return Result.Success;
    }

    private static Result<Success> ValidateBoundary(Point boundary)
    {
        if (boundary is null) return RestaurantErrors.BoundaryRequired;
        if (!boundary.IsValid) return RestaurantErrors.BoundaryInvalidCoordinates;

        return Result.Success;
    }
}