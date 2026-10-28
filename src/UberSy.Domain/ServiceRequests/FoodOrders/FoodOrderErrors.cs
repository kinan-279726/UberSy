using UberSy.Domain.Common.Results;

namespace UberSy.Domain.ServiceRequests.FoodOrders;

public static class FoodOrderErrors
{
    public static Error RestaurantIdRequired =>
        Error.Validation("Restaurant_Id_Required", "Restaurant Id Is Required");

    public static Error AddressIdRequired =>
        Error.Validation("Address_Id_Required", "Address Id Is Required");

    public static Error DiscountAmountNegative =>
        Error.Validation("Discount_Amount_Negative", "Discount Amount Cannot Be Negative");

    public static Error DeliveryFeeNegative =>
        Error.Validation("Delivery_Fee_Negative", "Delivery Fee Cannot Be Negative");

    public static Error DriverEarningNegative =>
        Error.Validation("Driver_Earning_Negative", "Driver Earning Cannot Be Negative");

    public static Error DriverEarningExceedsDeliveryFee =>
        Error.Validation("Driver_Earning_Exceeds_Delivery_Fee", "Driver Earning Cannot Exceed The Delivery Fee");

    public static Error CommissionNegative =>
        Error.Validation("Commission_Negative", "Platform Commission Cannot Be Negative");

    public static Error DriverAlreadyAssigned =>
        Error.Conflict("Driver_Already_Assigned", "A Driver Is Already Assigned To This Order");

    public static Error NoDriverAssigned =>
        Error.Conflict("No_Driver_Assigned", "No Driver Is Currently Assigned To This Order");
}