using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Restaurants.Categories;

public static class CategoryErrors
{
    // ================================================================
    // Validation
    // ================================================================

    // ============ RestaurantId ============
    public static Error RestaurantIdRequired =>
        Error.Validation("Restaurant_Id_Required", "Restaurant Id Is Required");

    public static Error RestaurantIdInvalid =>
        Error.Validation("Restaurant_Id_Invalid", "Restaurant Id Is Invalid");

    // ============ PrinterId ============
    public static Error PrinterIdInvalid =>
        Error.Validation("Printer_Id_Invalid", "Printer Id Is Invalid");

    public static Error PrinterNotBelongToRestaurant =>
        Error.Validation("Printer_Not_Belong_To_Restaurant", "Printer Does Not Belong To This Restaurant");

    public static Error PrinterInactive =>
        Error.Validation("Printer_Inactive", "Cannot Assign An Inactive Printer");

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

    // ============ SortOrder ============
    public static Error SortOrderInvalid =>
        Error.Validation("Sort_Order_Invalid", "Sort Order Is Invalid");

    public static Error SortOrderNegative =>
        Error.Validation("Sort_Order_Negative", "Sort Order Cannot Be Negative");

    public static Error SortOrderAlreadyExists =>
        Error.Validation("Sort_Order_Already_Exists", "Another Category Already Uses This Sort Order");

    public static Error SortOrderDuplicate =>
        Error.Validation("Sort_Order_Duplicate", "Sort Order Must Be Unique Within The Same Restaurant");

    // ============ IsActive ============
    public static Error IsActiveInvalid =>
        Error.Validation("Is_Active_Invalid", "Is Active Value Is Invalid");

    // ================================================================
    // Conflict
    // ================================================================

    public static Error NameArAlreadyExists =>
        Error.Conflict("Name_Ar_Already_Exists", "A Category With This Arabic Name Already Exists In This Restaurant");

    public static Error NameEnAlreadyExists =>
        Error.Conflict("Name_En_Already_Exists", "A Category With This English Name Already Exists In This Restaurant");

    public static Error CategoryAlreadyActive =>
        Error.Conflict("Category_Already_Active", "Category Is Already Active");

    public static Error CategoryAlreadyInactive =>
        Error.Conflict("Category_Already_Inactive", "Category Is Already Inactive");

    public static Error CategoryInUse =>
        Error.Conflict("Category_In_Use", "Category Cannot Be Deleted Because It Is In Use");

    public static Error CategoryHasItems =>
        Error.Conflict("Category_Has_Items", "Category Cannot Be Deleted While It Has Menu Items");

    public static Error CategoryHasActiveItems =>
        Error.Conflict("Category_Has_Active_Items", "Category Cannot Be Deactivated While It Has Active Items");

    public static Error CannotDeactivateLastActiveCategory =>
        Error.Conflict("Cannot_Deactivate_Last_Active_Category", "Cannot Deactivate The Last Active Category In The Restaurant");

    // ================================================================
    // NotFound
    // ================================================================

    public static Error CategoryNotFound =>
        Error.NotFound("Category_Not_Found", "Category Not Found");

    public static Error RestaurantNotFound =>
        Error.NotFound("Restaurant_Not_Found", "Restaurant Not Found");

    public static Error PrinterNotFound =>
        Error.NotFound("Printer_Not_Found", "Printer Not Found");

    // ================================================================
    // Forbidden
    // ================================================================

    public static Error NotCategoryOwner =>
        Error.Forbidden("Not_Category_Owner", "You Do Not Own This Category");

    public static Error RestaurantNotApproved =>
        Error.Forbidden("Restaurant_Not_Approved", "Restaurant Is Not Approved Yet");

    public static Error RestaurantSuspended =>
        Error.Forbidden("Restaurant_Suspended", "Restaurant Has Been Suspended");

    // ================================================================
    // Failure
    // ================================================================

    public static Error CategoryCreationFailed =>
        Error.Failure("Category_Creation_Failed", "Failed To Create Category");

    public static Error CategoryUpdateFailed =>
        Error.Failure("Category_Update_Failed", "Failed To Update Category");

    public static Error CategoryDeletionFailed =>
        Error.Failure("Category_Deletion_Failed", "Failed To Delete Category");

    // ================================================================
    // Unexpected
    // ================================================================

    public static Error UnexpectedError =>
        Error.Unexpected("Unexpected_Error", "An Unexpected Error Occurred While Processing Category");
}
