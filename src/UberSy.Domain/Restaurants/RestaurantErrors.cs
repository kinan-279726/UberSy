using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Restaurants;

public static class RestaurantErrors
{
    // ================================================================
    // Validation
    // ================================================================

    // ============ OwnerId ============
    public static Error OwnerIdRequired =>
        Error.Validation("Owner_Id_Required", "Owner Id Is Required");

    public static Error OwnerIdInvalid =>
        Error.Validation("Owner_Id_Invalid", "Owner Id Is Invalid");

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
    public static Error DescriptionArTooLong =>
        Error.Validation("Description_Ar_Too_Long", "Arabic Description Cannot Exceed 1000 Characters");

    // ============ DescriptionEn ============
    public static Error DescriptionEnTooLong =>
        Error.Validation("Description_En_Too_Long", "English Description Cannot Exceed 1000 Characters");

    // ============ Phone ============
    public static Error PhoneRequired =>
        Error.Validation("Phone_Required", "Phone Is Required");

    public static Error PhoneInvalid =>
        Error.Validation("Phone_Invalid", "Phone number must start with +9639 and end with 8 digits");

    public static Error PhoneTooLong =>
        Error.Validation("Phone_Too_Long", "Phone Cannot Exceed 20 Characters");

    // ============ Email ============
    public static Error EmailInvalid =>
        Error.Validation("Email_Invalid", "Email Format Is Invalid");

    public static Error EmailTooLong =>
        Error.Validation("Email_Too_Long", "Email Cannot Exceed 100 Characters");

    // ============ LogoUrl ============
    public static Error LogoUrlInvalid =>
        Error.Validation("Logo_Url_Invalid", "Logo Url Is Invalid");

    public static Error LogoUrlTooLong =>
        Error.Validation("Logo_Url_Too_Long", "Logo Url Cannot Exceed 1000 Characters");

    // ============ CoverUrl ============
    public static Error CoverUrlInvalid =>
        Error.Validation("Cover_Url_Invalid", "Cover Url Is Invalid");

    public static Error CoverUrlTooLong =>
        Error.Validation("Cover_Url_Too_Long", "Cover Url Cannot Exceed 1000 Characters");

    // ============ CommissionRate ============
    public static Error CommissionRateRequired =>
        Error.Validation("Commission_Rate_Required", "Commission Rate Is Required");

    public static Error CommissionRateInvalid =>
        Error.Validation("Commission_Rate_Invalid", "Commission Rate Must Be Between 1% And 15%");

    public static Error CommissionRateNegative =>
        Error.Validation("Commission_Rate_Negative", "Commission Rate Cannot Be Negative");

    public static Error CommissionRateExceedsMaximum =>
        Error.Validation("Commission_Rate_Exceeds_Maximum", "Commission Rate Cannot Exceed 100 Percent");

    // ============ TaxRate ============
    public static Error TaxRateInvalid =>
        Error.Validation("Tax_Rate_Invalid", "Tax Rate Must Be Between 0 And 100");

    public static Error TaxRateNegative =>
        Error.Validation("Tax_Rate_Negative", "Tax Rate Cannot Be Negative");

    public static Error TaxRateExceedsMaximum =>
        Error.Validation("Tax_Rate_Exceeds_Maximum", "Tax Rate Cannot Exceed 100 Percent");

    // ============ OpeningTime ============
    public static Error OpeningTimeRequired =>
        Error.Validation("Opening_Time_Required", "Opening Time Is Required");

    public static Error OpeningTimeInvalid =>
        Error.Validation("Opening_Time_Invalid", "Opening Time Is Invalid");

    // ============ ClosingTime ============
    public static Error ClosingTimeRequired =>
        Error.Validation("Closing_Time_Required", "Closing Time Is Required");

    public static Error ClosingTimeInvalid =>
        Error.Validation("Closing_Time_Invalid", "Closing Time Is Invalid");

    public static Error ClosingTimeMustBeAfterOpeningTime =>
        Error.Validation("Closing_Time_Must_Be_After_Opening_Time", "Closing Time Must Be After Opening Time");

    // ============ MinOrderAmount ============
    public static Error MinOrderAmountRequired =>
        Error.Validation("Min_Order_Amount_Required", "Minimum Order Amount Is Required");

    public static Error MinOrderAmountNegative =>
        Error.Validation("Min_Order_Amount_Negative", "Minimum Order Amount Cannot Be Negative");

    public static Error MinOrderAmountTooSmall =>
        Error.Validation("Min_Order_Amount_Too_Small", "Minimum Order Amount Is Too Small");

    // ============ MaxOrderAmount ============
    public static Error MaxOrderAmountRequired =>
        Error.Validation("Max_Order_Amount_Required", "Maximum Order Amount Is Required");

    public static Error MaxOrderAmountNegative =>
        Error.Validation("Max_Order_Amount_Negative", "Maximum Order Amount Cannot Be Negative");

    public static Error MaxOrderAmountMustBeGreaterThanMin =>
        Error.Validation("Max_Order_Amount_Must_Be_Greater_Than_Min", "Maximum Order Amount Must Be Greater Than Minimum Order Amount");

    // ============ Boundary ============
    public static Error BoundaryRequired =>
        Error.Validation("Boundary_Required", "Boundary Is Required");

    public static Error BoundaryInvalid =>
        Error.Validation("Boundary_Invalid", "Boundary Must Be A Valid Geographic Shape");

    public static Error BoundaryTooSmall =>
        Error.Validation("Boundary_Too_Small", "Boundary Area Is Too Small To Be A Valid Restaurant Zone");

    public static Error BoundaryTooLarge =>
        Error.Validation("Boundary_Too_Large", "Boundary Area Exceeds The Maximum Allowed Size");

    public static Error BoundaryNotClosed =>
        Error.Validation("Boundary_Not_Closed", "Polygon Boundary Must Be Closed (First Point = Last Point)");

    public static Error BoundarySelfIntersects =>
        Error.Validation("Boundary_Self_Intersects", "Polygon Boundary Cannot Self-Intersect");

    public static Error BoundaryInvalidCoordinates =>
        Error.Validation("Boundary_Invalid_Coordinates", "Boundary Contains Invalid Coordinates");

    public static Error BoundaryOutsideServiceZone =>
        Error.Validation("Boundary_Outside_Service_Zone", "Restaurant Boundary Must Be Inside An Active Service Zone");

    // ============ Rating ============
    public static Error RatingInvalid =>
        Error.Validation("Rating_Invalid", "Rating Must Be Between 0 And 5");

    public static Error RatingNegative =>
        Error.Validation("Rating_Negative", "Rating Cannot Be Negative");

    public static Error RatingExceedsMaximum =>
        Error.Validation("Rating_Exceeds_Maximum", "Rating Cannot Exceed 5");

    // ============ IsOpenNow ============
    public static Error IsOpenNowInvalid =>
        Error.Validation("Is_Open_Now_Invalid", "Is Open Now Value Is Invalid");

    // ============ IsApproved ============
    public static Error IsApprovedInvalid =>
        Error.Validation("Is_Approved_Invalid", "Is Approved Value Is Invalid");

    // ============ AcceptReservation ============
    public static Error AcceptReservationInvalid =>
        Error.Validation("Accept_Reservation_Invalid", "Accept Reservation Value Is Invalid");

    // ================================================================
    // Conflict
    // ================================================================

    public static Error NameArAlreadyExists =>
        Error.Conflict("Name_Ar_Already_Exists", "A Restaurant With This Arabic Name Already Exists");

    public static Error NameEnAlreadyExists =>
        Error.Conflict("Name_En_Already_Exists", "A Restaurant With This English Name Already Exists");

    public static Error PhoneAlreadyExists =>
        Error.Conflict("Phone_Already_Exists", "A Restaurant With This Phone Already Exists");

    public static Error EmailAlreadyExists =>
        Error.Conflict("Email_Already_Exists", "A Restaurant With This Email Already Exists");

    public static Error OwnerAlreadyHasRestaurant =>
        Error.Conflict("Owner_Already_Has_Restaurant", "This Owner Already Has A Restaurant");

    public static Error RestaurantAlreadyApproved =>
        Error.Conflict("Restaurant_Already_Approved", "Restaurant Is Already Approved");

    public static Error RestaurantAlreadyRejected =>
        Error.Conflict("Restaurant_Already_Rejected", "Restaurant Is Already Rejected");

    public static Error RestaurantAlreadyOpen =>
        Error.Conflict("Restaurant_Already_Open", "Restaurant Is Already Open");

    public static Error RestaurantAlreadyClosed =>
        Error.Conflict("Restaurant_Already_Closed", "Restaurant Is Already Closed");

    public static Error RestaurantInUse =>
        Error.Conflict("Restaurant_In_Use", "Restaurant Cannot Be Deleted Because It Is In Use");

    public static Error RestaurantHasActiveOrders =>
        Error.Conflict("Restaurant_Has_Active_Orders", "Restaurant Cannot Be Deleted While It Has Active Orders");

    public static Error RestaurantHasActiveReservations =>
        Error.Conflict("Restaurant_Has_Active_Reservations", "Restaurant Cannot Be Deleted While It Has Active Reservations");

    public static Error RestaurantHasItems =>
        Error.Conflict("Restaurant_Has_Items", "Restaurant Cannot Be Deleted While It Has Menu Items");

    public static Error RestaurantHasActiveDiscounts =>
        Error.Conflict("Restaurant_Has_Active_Discounts", "Restaurant Cannot Be Deleted While It Has Active Discounts");

    public static Error RestaurantHasActiveStaff =>
        Error.Conflict("Restaurant_Has_Active_Staff", "Restaurant Cannot Be Deleted While It Has Active Staff");

    // ================================================================
    // NotFound
    // ================================================================

    public static Error RestaurantNotFound =>
        Error.NotFound("Restaurant_Not_Found", "Restaurant Not Found");

    public static Error OwnerNotFound =>
        Error.NotFound("Owner_Not_Found", "Owner Not Found");

    // ================================================================
    // Forbidden
    // ================================================================

    public static Error NotRestaurantOwner =>
        Error.Forbidden("Not_Restaurant_Owner", "You Do Not Own This Restaurant");

    public static Error RestaurantNotApproved =>
        Error.Forbidden("Restaurant_Not_Approved", "Restaurant Is Not Approved Yet");

    public static Error RestaurantSuspended =>
        Error.Forbidden("Restaurant_Suspended", "Restaurant Has Been Suspended");

    public static Error RestaurantClosed =>
        Error.Forbidden("Restaurant_Closed", "Restaurant Is Currently Closed");

    public static Error CannotModifyApprovedRestaurant =>
        Error.Forbidden("Cannot_Modify_Approved_Restaurant", "Cannot Modify An Approved Restaurant Without Admin Permission");

    // ================================================================
    // Failure
    // ================================================================

    public static Error RestaurantCreationFailed =>
        Error.Failure("Restaurant_Creation_Failed", "Failed To Create Restaurant");

    public static Error RestaurantUpdateFailed =>
        Error.Failure("Restaurant_Update_Failed", "Failed To Update Restaurant");

    public static Error RestaurantDeletionFailed =>
        Error.Failure("Restaurant_Deletion_Failed", "Failed To Delete Restaurant");

    public static Error LogoUploadFailed =>
        Error.Failure("Logo_Upload_Failed", "Failed To Upload Logo");

    public static Error CoverUploadFailed =>
        Error.Failure("Cover_Upload_Failed", "Failed To Upload Cover Image");

    // ================================================================
    // Unexpected
    // ================================================================

    public static Error UnexpectedError =>
        Error.Unexpected("Unexpected_Error", "An Unexpected Error Occurred While Processing Restaurant");
}
