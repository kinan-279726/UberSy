using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Restaurants.AddOns;

public static class AddOnErrors
{
    // ================================================================
    // Validation
    // ================================================================

    // ============ RestaurantId ============
    public static Error RestaurantIdRequired =>
        Error.Validation("Restaurant_Id_Required", "Restaurant Id Is Required");

    public static Error RestaurantIdInvalid =>
        Error.Validation("Restaurant_Id_Invalid", "Restaurant Id Is Invalid");

    // ============ NameAr ============
    public static Error NameArRequired =>
        Error.Validation("Name_Ar_Required", "Arabic Name Is Required");

    public static Error NameArTooLong =>
        Error.Validation("Name_Ar_Too_Long", "Arabic Name Cannot Exceed 100 Characters");

    public static Error NameArTooShort =>
        Error.Validation("Name_Ar_Too_Short", "Arabic Name Must Be At Least 2 Characters");

    // ============ NameEn ============
    public static Error NameEnRequired =>
        Error.Validation("Name_En_Required", "English Name Is Required");

    public static Error NameEnTooLong =>
        Error.Validation("Name_En_Too_Long", "English Name Cannot Exceed 100 Characters");

    public static Error NameEnTooShort =>
        Error.Validation("Name_En_Too_Short", "English Name Must Be At Least 2 Characters");

    // ============ Price ============
    public static Error PriceRequired =>
        Error.Validation("Price_Required", "Price Is Required");

    public static Error PriceNegative =>
        Error.Validation("Price_Negative", "Price Cannot Be Negative");

    public static Error PriceZero =>
        Error.Validation("Price_Zero", "Price Must Be Greater Than Zero");

    public static Error PriceTooLarge =>
        Error.Validation("Price_Too_Large", "Price Exceeds The Maximum Allowed Value");

    public static Error PriceExceedsRestaurantMaxOrder =>
        Error.Validation("Price_Exceeds_Restaurant_Max_Order", "Add-On Price Exceeds Restaurant Maximum Order Amount");

    // ============ IsActive ============
    public static Error IsActiveInvalid =>
        Error.Validation("Is_Active_Invalid", "Is Active Value Is Invalid");

    // ================================================================
    // Conflict
    // ================================================================

    public static Error NameArAlreadyExists =>
        Error.Conflict("Name_Ar_Already_Exists", "An Add-On With This Arabic Name Already Exists In This Restaurant");

    public static Error NameEnAlreadyExists =>
        Error.Conflict("Name_En_Already_Exists", "An Add-On With This English Name Already Exists In This Restaurant");

    public static Error AddOnAlreadyActive =>
        Error.Conflict("Add_On_Already_Active", "Add-On Is Already Active");

    public static Error AddOnAlreadyInactive =>
        Error.Conflict("Add_On_Already_Inactive", "Add-On Is Already Inactive");

    public static Error AddOnInUse =>
        Error.Conflict("Add_On_In_Use", "Add-On Cannot Be Deleted Because It Is In Use");

    public static Error AddOnHasItems =>
        Error.Conflict("Add_On_Has_Items", "Add-On Cannot Be Deleted While It Is Linked To Items");

    public static Error AddOnHasActiveItems =>
        Error.Conflict("Add_On_Has_Active_Items", "Add-On Cannot Be Deactivated While It Is Linked To Active Items");

    public static Error AddOnHasOrders =>
        Error.Conflict("Add_On_Has_Orders", "Add-On Cannot Be Deleted While It Has Orders");

    public static Error AddOnHasActiveOrders =>
        Error.Conflict("Add_On_Has_Active_Orders", "Add-On Cannot Be Deleted While It Has Active Orders");

    // ================================================================
    // NotFound
    // ================================================================

    public static Error AddOnNotFound =>
        Error.NotFound("Add_On_Not_Found", "Add-On Not Found");

    public static Error RestaurantNotFound =>
        Error.NotFound("Restaurant_Not_Found", "Restaurant Not Found");

    // ================================================================
    // Forbidden
    // ================================================================

    public static Error NotAddOnOwner =>
        Error.Forbidden("Not_Add_On_Owner", "You Do Not Own This Add-On");

    public static Error RestaurantNotApproved =>
        Error.Forbidden("Restaurant_Not_Approved", "Restaurant Is Not Approved Yet");

    public static Error RestaurantSuspended =>
        Error.Forbidden("Restaurant_Suspended", "Restaurant Has Been Suspended");

    // ================================================================
    // Failure
    // ================================================================

    public static Error AddOnCreationFailed =>
        Error.Failure("Add_On_Creation_Failed", "Failed To Create Add-On");

    public static Error AddOnUpdateFailed =>
        Error.Failure("Add_On_Update_Failed", "Failed To Update Add-On");

    public static Error AddOnDeletionFailed =>
        Error.Failure("Add_On_Deletion_Failed", "Failed To Delete Add-On");

    // ================================================================
    // Unexpected
    // ================================================================

    public static Error UnexpectedError =>
        Error.Unexpected("Unexpected_Error", "An Unexpected Error Occurred While Processing Add-On");
}
