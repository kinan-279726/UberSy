using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Restaurants.Printers;

public static class PrinterErrors
{
    // ================================================================
    // Validation
    // ================================================================

    // ============ RestaurantId ============
    public static Error RestaurantIdRequired =>
        Error.Validation("Restaurant_Id_Required", "Restaurant Id Is Required");

    public static Error RestaurantIdInvalid =>
        Error.Validation("Restaurant_Id_Invalid", "Restaurant Id Is Invalid");

    // ============ Type ============
    public static Error TypeRequired =>
        Error.Validation("Type_Required", "Printer Type Is Required");

    public static Error TypeInvalid =>
        Error.Validation("Type_Invalid", "Printer Type Must Be 1 (Kitchen), 2 (Cashier), or 3 (Bar)");

    // ============ Name ============
    public static Error NameTooLong =>
        Error.Validation("Name_Too_Long", "Printer Name Cannot Exceed 100 Characters");

    public static Error NameTooShort =>
        Error.Validation("Name_Too_Short", "Printer Name Must Be At Least 2 Characters");

    // ============ ConnectionInfo ============
    public static Error ConnectionInfoRequired =>
        Error.Validation("Connection_Info_Required", "Connection Info Is Required");

    public static Error ConnectionInfoTooLong =>
        Error.Validation("Connection_Info_Too_Long", "Connection Info Cannot Exceed 1000 Characters");

    public static Error ConnectionInfoInvalid =>
        Error.Validation("Connection_Info_Invalid", "Connection Info Format Is Invalid");

    public static Error ConnectionInfoInvalidIp =>
        Error.Validation("Connection_Info_Invalid_Ip", "Connection Info Contains An Invalid IP Address");

    public static Error ConnectionInfoInvalidPort =>
        Error.Validation("Connection_Info_Invalid_Port", "Connection Info Contains An Invalid Port");

    public static Error ConnectionInfoInvalidFormat =>
        Error.Validation("Connection_Info_Invalid_Format", "Connection Info Must Follow The Format: IP:Port Or Protocol://Address");

    // ============ IsActive ============
    public static Error IsActiveInvalid =>
        Error.Validation("Is_Active_Invalid", "Is Active Value Is Invalid");

    // ================================================================
    // Conflict
    // ================================================================

    public static Error PrinterAlreadyActive =>
        Error.Conflict("Printer_Already_Active", "Printer Is Already Active");

    public static Error PrinterAlreadyInactive =>
        Error.Conflict("Printer_Already_Inactive", "Printer Is Already Inactive");

    public static Error PrinterInUse =>
        Error.Conflict("Printer_In_Use", "Printer Cannot Be Deleted Because It Is In Use");

    public static Error PrinterHasCategories =>
        Error.Conflict("Printer_Has_Categories", "Printer Cannot Be Deleted While It Is Linked To Categories");

    public static Error PrinterHasActiveCategories =>
        Error.Conflict("Printer_Has_Active_Categories", "Printer Cannot Be Deactivated While It Is Linked To Active Categories");

    public static Error DuplicateConnectionInfo =>
        Error.Conflict("Duplicate_Connection_Info", "Another Printer In This Restaurant Already Uses This Connection Info");

    public static Error DuplicatePrinterTypeInRestaurant =>
        Error.Conflict("Duplicate_Printer_Type_In_Restaurant", "A Printer Of This Type Already Exists In This Restaurant");

    // ================================================================
    // NotFound
    // ================================================================

    public static Error PrinterNotFound =>
        Error.NotFound("Printer_Not_Found", "Printer Not Found");

    public static Error RestaurantNotFound =>
        Error.NotFound("Restaurant_Not_Found", "Restaurant Not Found");

    // ================================================================
    // Forbidden
    // ================================================================

    public static Error NotPrinterOwner =>
        Error.Forbidden("Not_Printer_Owner", "You Do Not Own This Printer");

    public static Error RestaurantNotApproved =>
        Error.Forbidden("Restaurant_Not_Approved", "Restaurant Is Not Approved Yet");

    public static Error RestaurantSuspended =>
        Error.Forbidden("Restaurant_Suspended", "Restaurant Has Been Suspended");

    // ================================================================
    // Failure
    // ================================================================

    public static Error PrinterCreationFailed =>
        Error.Failure("Printer_Creation_Failed", "Failed To Create Printer");

    public static Error PrinterUpdateFailed =>
        Error.Failure("Printer_Update_Failed", "Failed To Update Printer");

    public static Error PrinterDeletionFailed =>
        Error.Failure("Printer_Deletion_Failed", "Failed To Delete Printer");

    public static Error PrinterConnectionFailed =>
        Error.Failure("Printer_Connection_Failed", "Failed To Connect To Printer");

    public static Error PrinterTestFailed =>
        Error.Failure("Printer_Test_Failed", "Printer Test Print Failed");

    // ================================================================
    // Unexpected
    // ================================================================

    public static Error UnexpectedError =>
        Error.Unexpected("Unexpected_Error", "An Unexpected Error Occurred While Processing Printer");
}