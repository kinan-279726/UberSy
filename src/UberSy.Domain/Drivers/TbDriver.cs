using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.DeliveryRule.Enums;
using UberSy.Domain.Drivers.Enums;
using UberSy.Domain.Drivers.Wallet;
using UberSy.Domain.Reviews;
using UberSy.Domain.ServiceRequests.FoodOrders;
using UberSy.Domain.ServiceRequests.Offers;
using UberSy.Domain.ServiceRequests.RideRequests;

namespace UberSy.Domain.Drivers;

public sealed class TbDriver : AuditableEntity
{
    public Guid UserId { get; private set; }

    public VehicleType VehicleType { get; private set; }
    
    public ServiceTier ServiceTier { get; private set; }
    
    public CarType? CarType { get; private set; }
    
    public string NationalId { get; private set; } = null!;
    
    public string? PhotoUrl { get; private set; }
    
    public string? LicenseNumber { get; private set; }
    
    public string? PlateNumber { get; private set; }
    
    public string? Model { get; private set; }
    
    public string? Color { get; private set; }
    
    public DriverStatus Status { get; private set; }
    
    public bool IsApproved { get; private set; }

    public TbDriverLocation? DriverLocation { get; private set; }
    public TbDriverWallet? DriverWallet { get; private set; }

    private readonly List<TbFoodOrder> _foodOrders = [];
    public IReadOnlyCollection<TbFoodOrder> FoodOrders => _foodOrders.AsReadOnly();

    private readonly List<TbReview> _reviews = [];
    public IReadOnlyCollection<TbReview> Reviews => _reviews.AsReadOnly();

    private readonly List<TbServiceRequestOffer> _offers = [];
    public IReadOnlyCollection<TbServiceRequestOffer> Offers => _offers.AsReadOnly();

    private readonly List<TbRideRequest> _rideRequests = [];
    public IReadOnlyCollection<TbRideRequest> RideRequests => _rideRequests.AsReadOnly();


    private TbDriver()
    { }

    private TbDriver(Guid id, Guid userId, VehicleType vehicleType, ServiceTier serviceTier, CarType? carType,
        string nationalId, string? photoUrl, string? licenseNumber, string? plateNumber, string? model, string? color)
        : base(id)
    {
        UserId = userId;
        VehicleType = vehicleType;
        ServiceTier = serviceTier;
        CarType = carType;
        NationalId = nationalId;
        PhotoUrl = photoUrl;
        LicenseNumber = licenseNumber;
        PlateNumber = plateNumber;
        Model = model;
        Color = color;
        Status = DriverStatus.Offline;
        IsApproved = false;
    }

    public static Result<TbDriver> Create(Guid id, Guid userId, VehicleType vehicleType, ServiceTier serviceTier,
        CarType? carType, string nationalId, string? photoUrl, string? licenseNumber, string? plateNumber,
        string? model, string? color)
    {
        if (userId == Guid.Empty) return DriverErrors.UserIdRequired;

        var config = ValidateVehicleConfiguration(vehicleType, serviceTier, carType);
        if (config.IsError) return config.Errors;

        var docs = ValidateDocuments(vehicleType, nationalId, licenseNumber, plateNumber);
        if (docs.IsError) return docs.Errors;

        return new TbDriver(id, userId, vehicleType, serviceTier, carType, nationalId.Trim(), photoUrl?.Trim(),
            licenseNumber?.Trim(), plateNumber?.Trim(), model?.Trim(), color?.Trim());
    }

    public Result<Updated> UpdateVehicle(VehicleType vehicleType, ServiceTier serviceTier, CarType? carType,
        string? licenseNumber, string? plateNumber, string? model, string? color)
    {
        if (Status == DriverStatus.OnDelivery) return DriverErrors.CannotChangeVehicleWhileOnDelivery;

        var config = ValidateVehicleConfiguration(vehicleType, serviceTier, carType);
        if (config.IsError) return config.Errors;

        var docs = ValidateDocuments(vehicleType, NationalId, licenseNumber, plateNumber);
        if (docs.IsError) return docs.Errors;

        VehicleType = vehicleType;
        ServiceTier = serviceTier;
        CarType = carType;
        LicenseNumber = licenseNumber?.Trim();
        PlateNumber = plateNumber?.Trim();
        Model = model?.Trim();
        Color = color?.Trim();

        return Result.Updated;
    }

    public Result<Updated> UpdatePhoto(string? photoUrl)
    {
        if (photoUrl is { Length: > 1000 }) return DriverErrors.PhotoUrlTooLong;

        PhotoUrl = photoUrl;

        return Result.Updated;
    }

    public Result<Updated> Approve()
    {
        if (IsApproved) return DriverErrors.DriverAlreadyApproved;

        IsApproved = true;

        return Result.Updated;
    }

    public Result<Updated> Reject()
    {
        if (!IsApproved) return DriverErrors.DriverAlreadyRejected;

        IsApproved = false;

        return Result.Updated;
    }

    public Result<Updated> GoOnline()
    {
        if (!IsApproved) return DriverErrors.DriverNotApproved;
        if (Status == DriverStatus.OnDelivery) return DriverErrors.CannotChangeStatusWhileOnDelivery;
        if (Status == DriverStatus.Online) return DriverErrors.DriverAlreadyOnline;

        Status = DriverStatus.Online;

        return Result.Updated;
    }

    public Result<Updated> GoOffline()
    {
        if (Status == DriverStatus.OnDelivery) return DriverErrors.CannotChangeStatusWhileOnDelivery;
        if (Status == DriverStatus.Offline) return DriverErrors.DriverAlreadyOffline;

        Status = DriverStatus.Offline;

        return Result.Updated;
    }

    public Result<Updated> StartDelivery()
    {
        if (Status != DriverStatus.Online) return DriverErrors.DriverOffline;

        Status = DriverStatus.OnDelivery;

        return Result.Updated;
    }

    public Result<Updated> FinishDelivery()
    {
        if (Status != DriverStatus.OnDelivery) return DriverErrors.DriverOnDelivery;

        Status = DriverStatus.Online;

        return Result.Updated;
    }

    public Result<Updated> Stop()
    {
        if (Status == DriverStatus.OnDelivery) return DriverErrors.CannotChangeStatusWhileOnDelivery;
        if (Status == DriverStatus.Stopped) return DriverErrors.DriverAlreadyStopped;

        Status = DriverStatus.Stopped;

        return Result.Updated;
    }

    private static Result<Success> ValidateVehicleConfiguration(VehicleType vehicleType, ServiceTier serviceTier, CarType? carType)
    {
        if (!Enum.IsDefined(vehicleType)) return DriverErrors.VehicleTypeInvalid;
        if (!Enum.IsDefined(serviceTier)) return DriverErrors.ServiceTierInvalid;

        if (serviceTier == ServiceTier.Premium && vehicleType != VehicleType.Car)
            return DriverErrors.ServiceTierRequiresCar;

        if (vehicleType == VehicleType.Car && carType is null) return DriverErrors.CarTypeRequired;
        if (vehicleType != VehicleType.Car && carType is not null) return DriverErrors.CarTypeNotAllowed;
        if (carType is not null && !Enum.IsDefined(carType.Value)) return DriverErrors.CarTypeInvalid;

        if (serviceTier == ServiceTier.Premium && carType != DeliveryRule.Enums.CarType.Private)
            return DriverErrors.CarTypePremiumRequiresPrivate;

        return Result.Success;
    }

    private static Result<Success> ValidateDocuments(VehicleType vehicleType, string nationalId, string? licenseNumber, string? plateNumber)
    {
        if (string.IsNullOrWhiteSpace(nationalId)) return DriverErrors.NationalIdRequired;
        if (nationalId.Length > 20) return DriverErrors.NationalIdTooLong;

        if (vehicleType is VehicleType.Motorcycle or VehicleType.Car)
        {
            if (string.IsNullOrWhiteSpace(licenseNumber)) return DriverErrors.LicenseNumberRequired;
            if (string.IsNullOrWhiteSpace(plateNumber)) return DriverErrors.PlateNumberRequired;
        }
        else if (!string.IsNullOrWhiteSpace(plateNumber))
        {
            return DriverErrors.PlateNumberNotAllowed;
        }

        if (licenseNumber is { Length: > 50 }) return DriverErrors.LicenseNumberTooLong;
        if (plateNumber is { Length: > 20 }) return DriverErrors.PlateNumberTooLong;

        return Result.Success;
    }
}

