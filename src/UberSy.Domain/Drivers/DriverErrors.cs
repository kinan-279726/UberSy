using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Drivers;

public static class DriverErrors
{
    // ================================================================
    // Validation
    // ================================================================

    // ============ UserId ============
    public static Error UserIdRequired =>
        Error.Validation("User_Id_Required", "User Id Is Required");

    public static Error UserIdInvalid =>
        Error.Validation("User_Id_Invalid", "User Id Is Invalid");

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

    // ============ NationalId ============
    public static Error NationalIdRequired =>
        Error.Validation("National_Id_Required", "National Id Is Required");

    public static Error NationalIdTooLong =>
        Error.Validation("National_Id_Too_Long", "National Id Cannot Exceed 20 Characters");

    public static Error NationalIdInvalid =>
        Error.Validation("National_Id_Invalid", "National Id Format Is Invalid");

    public static Error NationalIdInvalidLength =>
        Error.Validation("National_Id_Invalid_Length", "National Id Must Be A Specific Number Of Digits");

    // ============ PhotoUrl ============
    public static Error PhotoUrlInvalid =>
        Error.Validation("Photo_Url_Invalid", "Photo Url Format Is Invalid");

    public static Error PhotoUrlTooLong =>
        Error.Validation("Photo_Url_Too_Long", "Photo Url Cannot Exceed 1000 Characters");

    // ============ LicenseNumber ============
    public static Error LicenseNumberTooLong =>
        Error.Validation("License_Number_Too_Long", "License Number Cannot Exceed 50 Characters");

    public static Error LicenseNumberInvalid =>
        Error.Validation("License_Number_Invalid", "License Number Format Is Invalid");

    public static Error LicenseNumberRequired =>
        Error.Validation("License_Number_Required", "License Number Is Required For Cars And Motorcycles");

    // ============ PlateNumber ============
    public static Error PlateNumberTooLong =>
        Error.Validation("Plate_Number_Too_Long", "Plate Number Cannot Exceed 20 Characters");

    public static Error PlateNumberInvalid =>
        Error.Validation("Plate_Number_Invalid", "Plate Number Format Is Invalid");

    public static Error PlateNumberRequired =>
        Error.Validation("Plate_Number_Required", "Plate Number Is Required For Cars And Motorcycles");

    public static Error PlateNumberNotAllowed =>
        Error.Validation("Plate_Number_Not_Allowed", "Plate Number Is Not Applicable For Bicycles");

    // ============ Model ============
    public static Error ModelTooLong =>
        Error.Validation("Model_Too_Long", "Model Cannot Exceed 100 Characters");

    // ============ Color ============
    public static Error ColorTooLong =>
        Error.Validation("Color_Too_Long", "Color Cannot Exceed 100 Characters");

    // ============ Status ============
    public static Error StatusRequired =>
        Error.Validation("Status_Required", "Driver Status Is Required");

    public static Error StatusInvalid =>
        Error.Validation("Status_Invalid", "Driver Status Must Be 1 (Offline), 2 (Online), 3 (On Delivery), or 4 (Stopped)");

    // ============ IsApproved ============
    public static Error IsApprovedInvalid =>
        Error.Validation("Is_Approved_Invalid", "Is Approved Value Is Invalid");

    // ================================================================
    // Conflict
    // ================================================================

    public static Error UserAlreadyDriver =>
        Error.Conflict("User_Already_Driver", "This User Is Already Registered As A Driver");

    public static Error NationalIdAlreadyExists =>
        Error.Conflict("National_Id_Already_Exists", "A Driver With This National Id Already Exists");

    public static Error LicenseNumberAlreadyExists =>
        Error.Conflict("License_Number_Already_Exists", "A Driver With This License Number Already Exists");

    public static Error PlateNumberAlreadyExists =>
        Error.Conflict("Plate_Number_Already_Exists", "A Driver With This Plate Number Already Exists");

    public static Error DriverAlreadyApproved =>
        Error.Conflict("Driver_Already_Approved", "Driver Is Already Approved");

    public static Error DriverAlreadyRejected =>
        Error.Conflict("Driver_Already_Rejected", "Driver Is Already Rejected");

    public static Error DriverAlreadyOnline =>
        Error.Conflict("Driver_Already_Online", "Driver Is Already Online");

    public static Error DriverAlreadyOffline =>
        Error.Conflict("Driver_Already_Offline", "Driver Is Already Offline");

    public static Error DriverAlreadyStopped =>
        Error.Conflict("Driver_Already_Stopped", "Driver Is Already Stopped");

    public static Error DriverInUse =>
        Error.Conflict("Driver_In_Use", "Driver Cannot Be Deleted Because It Is In Use");

    public static Error DriverHasActiveOrders =>
        Error.Conflict("Driver_Has_Active_Orders", "Driver Cannot Be Deleted While It Has Active Orders");

    public static Error DriverHasActiveRides =>
        Error.Conflict("Driver_Has_Active_Rides", "Driver Cannot Be Deleted While It Has Active Rides");

    public static Error DriverHasActiveOffers =>
        Error.Conflict("Driver_Has_Active_Offers", "Driver Cannot Be Deleted While It Has Active Offers");

    public static Error DriverHasWallet =>
        Error.Conflict("Driver_Has_Wallet", "Driver Cannot Be Deleted While It Has A Wallet");

    public static Error DriverHasTransactions =>
        Error.Conflict("Driver_Has_Transactions", "Driver Cannot Be Deleted While It Has Wallet Transactions");

    // ================================================================
    // NotFound
    // ================================================================

    public static Error DriverNotFound =>
        Error.NotFound("Driver_Not_Found", "Driver Not Found");

    public static Error UserNotFound =>
        Error.NotFound("User_Not_Found", "User Not Found");

    // ================================================================
    // Forbidden
    // ================================================================

    public static Error NotDriverOwner =>
        Error.Forbidden("Not_Driver_Owner", "You Do Not Own This Driver Account");

    public static Error DriverNotApproved =>
        Error.Forbidden("Driver_Not_Approved", "Driver Account Is Not Approved Yet");

    public static Error DriverSuspended =>
        Error.Forbidden("Driver_Suspended", "Driver Account Has Been Suspended");

    public static Error DriverStopped =>
        Error.Forbidden("Driver_Stopped", "Driver Account Has Been Stopped");

    public static Error DriverOffline =>
        Error.Forbidden("Driver_Offline", "Driver Is Currently Offline");

    public static Error DriverOnDelivery =>
        Error.Forbidden("Driver_On_Delivery", "Driver Is Currently On A Delivery");

    public static Error CannotChangeStatusWhileOnDelivery =>
        Error.Forbidden("Cannot_Change_Status_While_On_Delivery", "Cannot Change Status While Driver Is On A Delivery");

    public static Error CannotChangeVehicleWhileOnDelivery =>
        Error.Forbidden("Cannot_Change_Vehicle_While_On_Delivery", "Cannot Change Vehicle While Driver Is On A Delivery");

    // ================================================================
    // Failure
    // ================================================================

    public static Error DriverCreationFailed =>
        Error.Failure("Driver_Creation_Failed", "Failed To Create Driver");

    public static Error DriverUpdateFailed =>
        Error.Failure("Driver_Update_Failed", "Failed To Update Driver");

    public static Error DriverDeletionFailed =>
        Error.Failure("Driver_Deletion_Failed", "Failed To Delete Driver");

    public static Error PhotoUploadFailed =>
        Error.Failure("Photo_Upload_Failed", "Failed To Upload Driver Photo");

    public static Error LicenseUploadFailed =>
        Error.Failure("License_Upload_Failed", "Failed To Upload License Document");

    public static Error DriverApprovalFailed =>
        Error.Failure("Driver_Approval_Failed", "Failed To Approve Driver");

    // ================================================================
    // Unexpected
    // ================================================================

    public static Error UnexpectedError =>
        Error.Unexpected("Unexpected_Error", "An Unexpected Error Occurred While Processing Driver");
}