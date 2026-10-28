using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Restaurants.Items;

public static class ItemErrors
{
    // ================================================================
    // Validation
    // ================================================================
    public static Error AddOnFromDifferentRestaurant =>
    Error.Validation("AddOn_From_Different_Restaurant", "Add-On Belongs To A Different Restaurant");

    // ============ RestaurantId ============
    public static Error RestaurantIdRequired =>
        Error.Validation("Restaurant_Id_Required", "Restaurant Id Is Required");

    public static Error RestaurantIdInvalid =>
        Error.Validation("Restaurant_Id_Invalid", "Restaurant Id Is Invalid");

    // ============ CategoryId ============
    public static Error CategoryIdRequired =>
        Error.Validation("Category_Id_Required", "Category Id Is Required");

    public static Error CategoryIdInvalid =>
        Error.Validation("Category_Id_Invalid", "Category Id Is Invalid");

    public static Error CategoryNotBelongToRestaurant =>
        Error.Validation("Category_Not_Belong_To_Restaurant", "Category Does Not Belong To This Restaurant");

    public static Error CategoryInactive =>
        Error.Validation("Category_Inactive", "Cannot Add Item To An Inactive Category");

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

    // ============ DescriptionAr ============
    public static Error DescriptionArRequired =>
        Error.Validation("Description_Ar_Required", "Arabic Description Is Required");

    public static Error DescriptionArTooLong =>
        Error.Validation("Description_Ar_Too_Long", "Arabic Description Cannot Exceed 1000 Characters");

    // ============ DescriptionEn ============
    public static Error DescriptionEnRequired =>
        Error.Validation("Description_En_Required", "English Description Is Required");

    public static Error DescriptionEnTooLong =>
        Error.Validation("Description_En_Too_Long", "English Description Cannot Exceed 1000 Characters");

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
        Error.Validation("Price_Exceeds_Restaurant_Max_Order", "Item Price Exceeds Restaurant Maximum Order Amount");

    public static Error PriceLessThanMinOrder =>
        Error.Validation("Price_Less_Than_Min_Order", "Item Price Is Less Than Restaurant Minimum Order Amount");

    // ============ ImageUrl ============
    public static Error ImageUrlRequired =>
        Error.Validation("Image_Url_Required", "Image Url Is Required");

    public static Error ImageUrlInvalid =>
        Error.Validation("Image_Url_Invalid", "Image Url Format Is Invalid");

    public static Error ImageUrlTooLong =>
        Error.Validation("Image_Url_Too_Long", "Image Url Cannot Exceed 1000 Characters");

    // ============ Calories ============
    public static Error CaloriesInvalid =>
        Error.Validation("Calories_Invalid", "Calories Value Is Invalid");

    public static Error CaloriesNegative =>
        Error.Validation("Calories_Negative", "Calories Cannot Be Negative");

    public static Error CaloriesTooLarge =>
        Error.Validation("Calories_Too_Large", "Calories Exceeds The Maximum Allowed Value");

    // ============ SortOrder ============
    public static Error SortOrderInvalid =>
        Error.Validation("Sort_Order_Invalid", "Sort Order Is Invalid");

    public static Error SortOrderNegative =>
        Error.Validation("Sort_Order_Negative", "Sort Order Cannot Be Negative");

    public static Error SortOrderDuplicate =>
        Error.Validation("Sort_Order_Duplicate", "Sort Order Must Be Unique Within The Same Category");

    // ============ IsActive ============
    public static Error IsActiveInvalid =>
        Error.Validation("Is_Active_Invalid", "Is Active Value Is Invalid");

    // ================================================================
    // Conflict
    // ================================================================

    public static Error NameArAlreadyExists =>
        Error.Conflict("Name_Ar_Already_Exists", "An Item With This Arabic Name Already Exists In This Restaurant");

    public static Error NameEnAlreadyExists =>
        Error.Conflict("Name_En_Already_Exists", "An Item With This English Name Already Exists In This Restaurant");

    public static Error ItemAlreadyActive =>
        Error.Conflict("Item_Already_Active", "Item Is Already Active");

    public static Error ItemAlreadyInactive =>
        Error.Conflict("Item_Already_Inactive", "Item Is Already Inactive");

    public static Error ItemOutOfStock =>
        Error.Conflict("Item_Out_Of_Stock", "Item Is Currently Out Of Stock");

    public static Error ItemInUse =>
        Error.Conflict("Item_In_Use", "Item Cannot Be Deleted Because It Is In Use");

    public static Error ItemHasOrders =>
        Error.Conflict("Item_Has_Orders", "Item Cannot Be Deleted While It Has Orders");

    public static Error ItemHasActiveOrders =>
        Error.Conflict("Item_Has_Active_Orders", "Item Cannot Be Deleted While It Has Active Orders");

    public static Error ItemHasActiveDiscounts =>
        Error.Conflict("Item_Has_Active_Discounts", "Item Cannot Be Deleted While It Has Active Discounts");

    public static Error ItemHasAddOns =>
        Error.Conflict("Item_Has_AddOns", "Item Cannot Be Deleted While It Has Add-Ons");

    public static Error AddOnAlreadyAssigned =>
    Error.Conflict("AddOn_Already_Assigned", "Add-On Is Already Assigned To This Item");

    public static Error CannotDeactivateLastActiveItemInCategory =>
        Error.Conflict("Cannot_Deactivate_Last_Active_Item_In_Category", "Cannot Deactivate The Last Active Item In The Category");

    // ================================================================
    // NotFound
    // ================================================================

    public static Error ItemNotFound =>
        Error.NotFound("Item_Not_Found", "Item Not Found");

    public static Error CategoryNotFound =>
        Error.NotFound("Category_Not_Found", "Category Not Found");

    public static Error RestaurantNotFound =>
        Error.NotFound("Restaurant_Not_Found", "Restaurant Not Found");

    public static Error AddOnNotAssigned =>
    Error.NotFound("AddOn_Not_Assigned", "Add-On Is Not Assigned To This Item");

    // ================================================================
    // Forbidden
    // ================================================================

    public static Error NotItemOwner =>
        Error.Forbidden("Not_Item_Owner", "You Do Not Own This Item");

    public static Error RestaurantNotApproved =>
        Error.Forbidden("Restaurant_Not_Approved", "Restaurant Is Not Approved Yet");

    public static Error RestaurantSuspended =>
        Error.Forbidden("Restaurant_Suspended", "Restaurant Has Been Suspended");

    // ================================================================
    // Failure
    // ================================================================

    public static Error ItemCreationFailed =>
        Error.Failure("Item_Creation_Failed", "Failed To Create Item");

    public static Error ItemUpdateFailed =>
        Error.Failure("Item_Update_Failed", "Failed To Update Item");

    public static Error ItemDeletionFailed =>
        Error.Failure("Item_Deletion_Failed", "Failed To Delete Item");

    public static Error ImageUploadFailed =>
        Error.Failure("Image_Upload_Failed", "Failed To Upload Item Image");

    // ================================================================
    // Unexpected
    // ================================================================

    public static Error UnexpectedError =>
        Error.Unexpected("Unexpected_Error", "An Unexpected Error Occurred While Processing Item");
}