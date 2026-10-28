using UberSy.Domain.Common.Results;

namespace UberSy.Domain.DeliveryRule;

public static class DeliveryRuleErrors
{
    // ============ VehicleType ============
    public static Error VehicleTypeRequired =>
        Error.Validation("Vehicle_Type_Required", "Vehicle Type Is Required");

    public static Error VehicleTypeInvalid =>
        Error.Validation("Vehicle_Type_Invalid", "Vehicle Type Must Be 1 (Motorcycle), 2 (Bicycle), or 3 (Car)");

    // ============ ServiceTier ============
    public static Error ServiceTierRequired =>
        Error.Validation("Service_Tier_Required", "Service Tier Is Required");

    public static Error ServiceTierInvalid =>
        Error.Validation("Service_Tier_Invalid", "Service Tier Must Be 1 (Standard) or 2 (Premium)");

    public static Error ServiceTierRequiresCar =>
        Error.Validation("Service_Tier_Requires_Car", "Premium Service Tier Is Only Available For Cars");

    // ============ CarType ============
    public static Error CarTypeRequired =>
        Error.Validation("Car_Type_Required", "Car Type Is Required When Vehicle Type Is Car");

    public static Error CarTypeNotAllowed =>
        Error.Validation("Car_Type_Not_Allowed", "Car Type Is Only Allowed When Vehicle Type Is Car");

    public static Error CarTypeInvalid =>
        Error.Validation("Car_Type_Invalid", "Car Type Must Be 1 (Taxi) or 2 (Private)");

    public static Error CarTypePremiumRequiresPrivate =>
        Error.Validation("Car_Type_Premium_Requires_Private", "Premium Service Tier Requires Private Car Type");

    // ============ CustomerRatePerKm ============
    public static Error CustomerRatePerKmRequired =>
        Error.Validation("Customer_Rate_Per_Km_Required", "Customer Rate Per Km Is Required");

    public static Error CustomerRatePerKmInvalid =>
        Error.Validation("Customer_Rate_Per_Km_Invalid", "Customer Rate Per Km Must Be Greater Than Zero");

    // ============ DriverRatePerKm ============
    public static Error DriverRatePerKmRequired =>
        Error.Validation("Driver_Rate_Per_Km_Required", "Driver Rate Per Km Is Required");

    public static Error DriverRatePerKmInvalid =>
        Error.Validation("Driver_Rate_Per_Km_Invalid", "Driver Rate Per Km Must Be Greater Than Zero");

    public static Error DriverRatePerKmExceedsCustomer =>
        Error.Validation("Driver_Rate_Per_Km_Exceeds_Customer", "Driver Rate Per Km Cannot Exceed Customer Rate Per Km");

    // ============ DriverRatePerOrder ============
    public static Error DriverRatePerOrderInvalid =>
        Error.Validation("Driver_Rate_Per_Order_Invalid", "Driver Rate Per Order Must Be Greater Than Zero");

    // ============ MaxDistanceKm ============
    public static Error MaxDistanceKmInvalid =>
        Error.Validation("Max_Distance_Km_Invalid", "Max Distance Km Must Be Greater Than Zero");

    // ============ StartHour ============
    public static Error StartHourInvalid =>
        Error.Validation("Start_Hour_Invalid", "Start Hour Must Be A Valid Time");

    // ============ EndHour ============
    public static Error EndHourInvalid =>
        Error.Validation("End_Hour_Invalid", "End Hour Must Be A Valid Time");

    public static Error EndHourMustBeAfterStartHour =>
        Error.Validation("End_Hour_Must_Be_After_Start_Hour", "End Hour Must Be After Start Hour");

    public static Error StartAndEndHourMustBothBeSet =>
        Error.Validation("Start_And_End_Hour_Must_Both_Be_Set", "Start Hour And End Hour Must Both Be Set Or Both Be Null");

    // ============ IsActive ============
    public static Error IsActiveRequired =>
        Error.Validation("Is_Active_Required", "Is Active Is Required");

    // ============ General ============
    public static Error DeliveryPricingRuleNotFound =>
        Error.NotFound("Delivery_Pricing_Rule_Not_Found", "Delivery Pricing Rule Not Found");

    public static Error DeliveryPricingRuleAlreadyExists =>
        Error.Conflict("Delivery_Pricing_Rule_Already_Exists", "A Delivery Pricing Rule With The Same Configuration Already Exists");
}
