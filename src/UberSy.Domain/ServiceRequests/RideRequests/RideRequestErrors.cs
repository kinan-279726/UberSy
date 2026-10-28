using UberSy.Domain.Common.Results;

namespace UberSy.Domain.ServiceRequests.RideRequests;

public static class RideRequestErrors
{
    public static Error VehicleTypeInvalid =>
        Error.Validation("Vehicle_Type_Invalid", "Requested Vehicle Type Is Invalid");

    public static Error ServiceTierInvalid =>
        Error.Validation("Service_Tier_Invalid", "Requested Service Tier Is Invalid");

    public static Error ServiceTierRequiresCar =>
        Error.Validation("Service_Tier_Requires_Car", "Premium Service Tier Is Only Available For Cars");

    public static Error CarTypeRequired =>
        Error.Validation("Car_Type_Required", "Car Type Is Required When Vehicle Is A Car");

    public static Error CarTypeNotAllowed =>
        Error.Validation("Car_Type_Not_Allowed", "Car Type Is Only Allowed When Vehicle Is A Car");

    public static Error CarTypePremiumRequiresPrivate =>
        Error.Validation("Car_Type_Premium_Requires_Private", "Premium Service Tier Requires Private Car Type");

    public static Error DistanceZero =>
        Error.Validation("Distance_Zero", "Distance Must Be Greater Than Zero");

    public static Error DeliveryFeeNegative =>
        Error.Validation("Delivery_Fee_Negative", "Delivery Fee Cannot Be Negative");

    public static Error DriverEarningExceedsDeliveryFee =>
        Error.Validation("Driver_Earning_Exceeds_Delivery_Fee", "Driver Earning Cannot Exceed The Delivery Fee");

    public static Error ReceiverPhoneTooLong =>
        Error.Validation("Receiver_Phone_Too_Long", "Receiver Phone Cannot Exceed 20 Characters");

    public static Error DriverAlreadyAssigned =>
        Error.Conflict("Driver_Already_Assigned", "A Driver Is Already Assigned To This Ride");
}