using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.DeliveryRule.Enums;

namespace UberSy.Domain.DeliveryRule;

public sealed class TbDeliveryPricingRule : AuditableEntity
{

    public VehicleType VehicleType { get; private set; }

    public ServiceTier ServiceTier { get; private set; }

    public CarType? CarType { get; private set; }

    public decimal CustomerRatePerKm { get; private set; }

    public decimal DriverRatePerKm { get; private set; }

    public decimal? DriverRatePerOrder { get; private set; }

    public decimal? MaxDistanceKm { get; private set; }

    public TimeOnly? StartHour { get; private set; }

    public TimeOnly? EndHour { get; private set; }

    public bool IsActive { get; private set; }

    private TbDeliveryPricingRule()
    {}

    private TbDeliveryPricingRule(
        Guid id,
        VehicleType vehicleType,
        ServiceTier serviceTier,
        CarType? carType,
        decimal customerRatePerKm,
        decimal driverRatePerKm,
        decimal? driverRatePerOrder,
        decimal? maxDistanceKm,
        TimeOnly? startHour,
        TimeOnly? endHour)
        : base(id)
    {
        VehicleType =  vehicleType;
        ServiceTier =  serviceTier;
        CarType = carType.HasValue ? carType.Value : null;
        CustomerRatePerKm = customerRatePerKm;
        DriverRatePerKm = driverRatePerKm;
        DriverRatePerOrder = driverRatePerOrder;
        MaxDistanceKm = maxDistanceKm;
        StartHour = startHour;
        EndHour = endHour;
        IsActive = true;
    }

    public static Result<TbDeliveryPricingRule> Create(
        Guid id,
        VehicleType vehicleType,
        ServiceTier serviceTier,
        CarType? carType,
        decimal customerRatePerKm,
        decimal driverRatePerKm,
        decimal? driverRatePerOrder,
        decimal? maxDistanceKm,
        TimeOnly? startHour,
        TimeOnly? endHour)
    {
        var validation = ValidateConfiguration(vehicleType, serviceTier, carType);
        if (validation.IsError)
        {
            return validation.Errors;
        }
       
        var rates = ValidateRates(customerRatePerKm, driverRatePerKm, driverRatePerOrder, maxDistanceKm);
        if (rates.IsError)
        {
            return rates.Errors;
        }

        var schedule = ValidateSchedule(startHour, endHour);
        if (schedule.IsError)
        {
            return schedule.Errors;
        }

        return new TbDeliveryPricingRule(id, vehicleType, serviceTier, carType, customerRatePerKm,
            driverRatePerKm, driverRatePerOrder, maxDistanceKm, startHour, endHour);
    }

    public Result<Updated> UpdateRates(decimal customerRatePerKm, decimal driverRatePerKm, decimal? driverRatePerOrder)
    {
        var result = ValidateRates(customerRatePerKm, driverRatePerKm, driverRatePerOrder, MaxDistanceKm);
        if (result.IsError)
        {
            return result.Errors;
        }

        CustomerRatePerKm = customerRatePerKm;
        DriverRatePerKm = driverRatePerKm;
        DriverRatePerOrder = driverRatePerOrder;

        return Result.Updated;
    }

    public Result<Updated> UpdateMaxDistance(decimal? maxDistanceKm)
    {
        if (maxDistanceKm is <= 0)
        {
            return DeliveryRuleErrors.MaxDistanceKmInvalid;
        }

        MaxDistanceKm = maxDistanceKm;

        return Result.Updated;
    }

    public Result<Updated> UpdateSchedule(TimeOnly? startHour, TimeOnly? endHour)
    {
        var result = ValidateSchedule(startHour, endHour);
        if (result.IsError)
        {
            return result.Errors;
        }

        StartHour = startHour;
        EndHour = endHour;

        return Result.Updated;
    }

    public Result<Updated> Activate()
    {
        IsActive = true;

        return Result.Updated;
    }

    public Result<Updated> Deactivate()
    {
        IsActive = false;

        return Result.Updated;
    }

    private static Result<Success> ValidateConfiguration(VehicleType vehicleType, ServiceTier serviceTier, CarType? carType)
    {
        if (!Enum.IsDefined(vehicleType))
        {
            return DeliveryRuleErrors.VehicleTypeInvalid;
        }

        if (!Enum.IsDefined(serviceTier))
        {
            return DeliveryRuleErrors.ServiceTierInvalid;
        }

        if (serviceTier == Enums.ServiceTier.Premium && vehicleType != Enums.VehicleType.Car)
        {
            return DeliveryRuleErrors.ServiceTierRequiresCar;
        }

        if (vehicleType == Enums.VehicleType.Car && carType is null)
        {
            return DeliveryRuleErrors.CarTypeRequired;
        }

        if (vehicleType != Enums.VehicleType.Car && carType is not null)
        {
            return DeliveryRuleErrors.CarTypeNotAllowed;
        }

        if (carType is not null && !Enum.IsDefined(carType.Value))
        {
            return DeliveryRuleErrors.CarTypeInvalid;
        }

        if (serviceTier == Enums.ServiceTier.Premium && carType != Enums.CarType.Private)
        {
            return DeliveryRuleErrors.CarTypePremiumRequiresPrivate;
        }

        return Result.Success;
    }

    private static Result<Success> ValidateRates(decimal customerRatePerKm, decimal driverRatePerKm, decimal? driverRatePerOrder, decimal? maxDistanceKm)
    {
        if (customerRatePerKm <= 0)
        {
            return DeliveryRuleErrors.CustomerRatePerKmInvalid;
        }

        if (driverRatePerKm <= 0)
        {
            return DeliveryRuleErrors.DriverRatePerKmInvalid;
        }

        if (driverRatePerKm > customerRatePerKm)
        {
            return DeliveryRuleErrors.DriverRatePerKmExceedsCustomer;
        }

        if (driverRatePerOrder is <= 0)
        {
            return DeliveryRuleErrors.DriverRatePerOrderInvalid;
        }

        if (maxDistanceKm is <= 0)
        {
            return DeliveryRuleErrors.MaxDistanceKmInvalid;
        }

        return Result.Success;
    }

    private static Result<Success> ValidateSchedule(TimeOnly? startHour, TimeOnly? endHour)
    {
        if (startHour is null != endHour is null)
        {
            return DeliveryRuleErrors.StartAndEndHourMustBothBeSet;
        }

        if (startHour is not null && endHour is not null && endHour <= startHour)
        {
            return DeliveryRuleErrors.EndHourMustBeAfterStartHour;
        }

        return Result.Success;
    }
}
