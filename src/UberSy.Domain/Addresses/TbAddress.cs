using NetTopologySuite.Geometries;
using System.Text.RegularExpressions;
using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.FoodOrders;

namespace UberSy.Domain.Addresses;

public sealed class TbAddress : Entity
{
    public Guid UserId { get; private set; }

    public string Street { get; private set; } = null!;

    public string City { get; private set; } = null!;

    public Point Boundary { get; private set; } = null!;

    public string? Label { get; private set; }

    public string? Floor { get; private set; }

    public string? Phone { get; private set; }

    public bool IsDefault { get; private set; }

    private readonly List<TbFoodOrder> _foodOrders = [];
    public IReadOnlyCollection<TbFoodOrder> FoodOrders => _foodOrders.AsReadOnly();

    private TbAddress()
    { }

    private TbAddress(Guid id, Guid userId, string street, string city, Point boundary, string? label, string? floor, string? phone)
        : base(id)
    {
        UserId = userId;
        Street = street;
        City = city;
        Boundary = boundary;
        Label = label;
        Floor = floor;
        Phone = phone;
        IsDefault = false;
    }

    public static Result<TbAddress> Create(Guid id, Guid userId, string street, string city, Point boundary,
        string? label, string? floor, string? phone)
    {
        if (userId == Guid.Empty) return AddressErrors.UserIdRequired;

        var result = Validate(street, city, boundary, label, phone);
        if (result.IsError) return result.Errors;

        return new TbAddress(id, userId, street.Trim(), city.Trim(), boundary, label?.Trim(), floor?.Trim(), phone?.Trim());
    }

    public Result<Updated> UpdateDetails(string street, string city, Point boundary, string? label, string? floor, string? phone)
    {
        var result = Validate(street, city, boundary, label, phone);
        if (result.IsError) return result.Errors;

        Street = street.Trim();
        City = city.Trim();
        Boundary = boundary;
        Label = label?.Trim();
        Floor = floor?.Trim();
        Phone = phone?.Trim();

        return Result.Updated;
    }

    public Result<Updated> MarkAsDefault()
    {
        IsDefault = true;
        return Result.Updated;
    }

    public Result<Updated> UnmarkAsDefault()
    {
        IsDefault = false;
        return Result.Updated;
    }

    private static Result<Success> Validate(string street, string city, Point boundary, string? label, string? phone)
    {
        if (string.IsNullOrWhiteSpace(street)) return AddressErrors.StreetRequired;
        if (street.Length > 100) return AddressErrors.StreetTooLong;

        if (string.IsNullOrWhiteSpace(city)) return AddressErrors.CityRequired;
        if (city.Length > 100) return AddressErrors.CityTooLong;

        if (boundary is null) return AddressErrors.BoundaryRequired;
        if (!boundary.IsValid) return AddressErrors.BoundaryInvalid;

        if (label is { Length: > 100 }) return AddressErrors.LabelTooLong;
        if (!Regex.IsMatch((phone is null ? "" : phone), @"^\+9639?\d{8}$")) return AddressErrors.InvalidPhoneNumber;

        return Result.Success;
    }
}