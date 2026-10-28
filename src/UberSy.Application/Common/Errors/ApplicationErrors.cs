using UberSy.Domain.Common.Results;

namespace UberSy.Application.Common.Errors;

public static class ApplicationErrors
{
    // ================================================================
    // Restaurant Rules
    // ================================================================
    public static Error CannotReleaseOrderAfterPickup =>
    Error.Conflict("Order_Cannot_Be_Released",
        "The order cannot be released after it has been picked up.");

    public static Error AddOnNotAvailableForItem =>
    Error.Validation("AddOn_Not_Available_For_Item", "This AddOn Is Not Available For The Selected Item");

    public static Error UserBanned =>
        Error.Forbidden("User_Banned", "This account is suspended.");

    public static Error CannotBanSelf =>
        Error.Conflict("Cannot_Ban_Self", "You cannot ban your own account.");

    public static Error CannotBanAdmin =>
        Error.Conflict("Cannot_Ban_Admin", "Admin accounts cannot be banned.");

    public static Error PricingRuleScheduleOverlap =>
    Error.Conflict("Pricing_Rule_Schedule_Overlap",
        "Another active pricing rule for the same vehicle configuration overlaps with these hours.");

    public static Error DeliveryPricingRuleNotFound =>
    Error.NotFound("Delivery_Pricing_Rule_Not_Found", "No Active Delivery Pricing Rule Found For This Vehicle Type");

    public static Error DeliveryDistanceTooLong =>
        Error.Validation("Delivery_Distance_Too_Long", "Delivery Distance Exceeds The Limit For This Vehicle Type");

    public static Error InvalidVerificationCode =>
    Error.Validation("Invalid_Verification_Code", "The Verification Code Is Invalid Or Has Expired");

    public static Error TooManyAttempts =>
        Error.Conflict("Too_Many_Attempts", "Too Many Failed Attempts, Try Again Later");

    public static Error PhoneAlreadyConfirmed =>
        Error.Conflict("Phone_Already_Confirmed", "Phone Number Is Already Confirmed");

    public static Error VerificationFailed =>
        Error.Failure("Verification_Failed", "Could Not Complete The Verification");

    public static Error EmailSendFailed =>
        Error.Failure("Email_Send_Failed", "Could Not Send The Verification Email");

    public static Error SmsSendFailed =>
        Error.Failure("Sms_Send_Failed", "Could Not Send The Verification SMS");

    public static Error DriverAlreadyRegistered =>
    Error.Conflict("Driver_Already_Registered", "This Account Is Already Registered As A Driver");

    public static Error NationalIdAlreadyRegistered =>
        Error.Conflict("National_Id_Already_Registered", "A Driver With This National Id Already Exists");

    public static Error RoleAlreadyAssigned =>
    Error.Conflict("Role_Already_Assigned", "User Already Has This Role");

    public static Error RoleNotAssigned =>
        Error.Conflict("Role_Not_Assigned", "User Does Not Have This Role");

    public static Error CannotRemoveOwnAdminRole =>
        Error.Forbidden("Cannot_Remove_Own_Admin_Role", "You Cannot Remove Your Own Admin Role");

    public static Error LastAdminCannotBeRemoved =>
        Error.Conflict("Last_Admin_Cannot_Be_Removed", "The System Must Keep At Least One Admin");

    public static Error DriverNotFound =>
        Error.NotFound("Driver_Not_Found", "Driver Not Found");

    public static Error OfferNoLongerAvailable =>
        Error.Conflict("Offer_NoL_onger_Available", "Offer No Longer Available");

    public static Error OfferNotFound =>
        Error.NotFound("Offer_Not_Found", "Offer Not Found");

    public static Error AlreadyHasDriver =>
        Error.Conflict("AlreadyHasDriver", "ALready has Driver");
    public static Error RestaurantClosed =>
        Error.Validation("Restaurant_Closed", "Restaurant Is Currently Closed");

    public static Error RestaurantNotAcceptingOrders =>
        Error.Validation("Restaurant_Not_Accepting_Orders", "Restaurant Is Not Accepting Orders Right Now");

    public static Error RestaurantNotApproved =>
        Error.Validation("Restaurant_Not_Approved", "Restaurant Is Not Approved Yet");

    public static Error RestaurantSuspended =>
        Error.Validation("Restaurant_Suspended", "Restaurant Has Been Suspended");

    public static Error RestaurantNotFound =>
        Error.NotFound("Restaurant_NotFound", "Restaurant Not Found");

    public static Error RestaurantDoesNotDeliverToArea =>
        Error.Validation("Restaurant_Does_Not_Deliver_To_Area", "Restaurant Does Not Deliver To This Area");

    public static Error RestaurantOutsideServiceZone =>
        Error.Validation("Restaurant_Outside_Service_Zone", "Restaurant Is Outside Any Active Service Zone");

    public static Error AddressNotFound =>
        Error.NotFound("Address_NotFound", "Address Not Found");

    public static Error ItemNotInRestaurant =>
        Error.NotFound("Not_Found", "Item Not Found in Restaurant");

    public static Error AddOnNotInRestaurant =>
        Error.NotFound("Not_Found", "AddOn Not Found in Restaurant");

    public static Error ServiceRequestNotFound =>
        Error.NotFound("Service_Request_NotFound", "Service Resquest Not Found");

    public static Error NoAvailableDriverFound =>
        Error.NotFound("No_Available_Driver_Found", "No Available Driver Found");

    // ================================================================
    // Order Rules
    // ================================================================

    public static Error AlreadyDeducted =>
    Error.Conflict("Already_Deducted", "The commission has already been deducted from the wallet.");

    public static Error WalletNotFound =>
        Error.NotFound("Wallet_Not_Found", "Driver wallet not found.");

    public static Error CommissionNegative =>
        Error.Validation("Commission_Negative", "Commission amount cannot be negative.");

    public static Error CartIsEmpty =>
        Error.Validation("Cart_Is_Empty", "Cart Cannot Be Empty");

    public static Error BelowMinimumOrderAmount =>
        Error.Validation("Below_Minimum_Order_Amount", "Order Total Is Below The Restaurant Minimum Order Amount");

    public static Error AboveMaximumOrderAmount =>
        Error.Validation("Above_Maximum_Order_Amount", "Order Total Exceeds The Restaurant Maximum Order Amount");

    public static Error OrderAlreadyAccepted =>
        Error.Validation("Order_Already_Accepted", "Order Has Already Been Accepted");

    public static Error OrderAlreadyCancelled =>
        Error.Validation("Order_Already_Cancelled", "Order Has Already Been Cancelled");

    public static Error OrderAlreadyDelivered =>
        Error.Validation("Order_Already_Delivered", "Order Has Already Been Delivered");

    public static Error OrderCannotBeCancelled =>
        Error.Validation("Order_Cannot_Be_Cancelled", "Order Cannot Be Cancelled At This Stage");

    public static Error OrderOutsideDeliveryTime =>
        Error.Validation("Order_Outside_Delivery_Time", "Order Cannot Be Placed Outside Delivery Hours");

    public static Error OrderResponseDeadlineExpired =>
        Error.Validation("Order_Response_Deadline_Expired", "Restaurant Response Deadline Has Expired");

    public static Error OrderItemsNotAvailable =>
        Error.Validation("Order_Items_Not_Available", "One Or More Items Are Not Available Right Now");

    public static Error ItemNotFound =>
        Error.NotFound("Item_NotFound", "Item Not Found");

    public static Error ItemOutOfStock =>
        Error.Validation("Item_Out_Of_Stock", "One Or More Items Are Out Of Stock");

    // ================================================================
    // Driver Rules
    // ================================================================

    public static Error DriverNotApproved =>
        Error.Validation("Driver_Not_Approved", "Driver Is Not Approved Yet");

    public static Error DriverOffline =>
        Error.Validation("Driver_Offline", "Driver Is Currently Offline");

    public static Error DriverSuspended =>
        Error.Validation("Driver_Suspended", "Driver Account Has Been Suspended");

    public static Error DriverAlreadyOnDelivery =>
        Error.Validation("Driver_Already_On_Delivery", "Driver Is Already On Another Delivery");

    public static Error DriverOutsideServiceZone =>
        Error.Validation("Driver_Outside_Service_Zone", "Driver Is Outside The Service Zone");

    public static Error DriverInsufficientWalletBalance =>
        Error.Validation("Driver_Insufficient_Wallet_Balance", "Driver Wallet Balance Is Insufficient");

    public static Error DriverVehicleTypeMismatch =>
        Error.Validation("Driver_Vehicle_Type_Mismatch", "Driver Vehicle Type Does Not Match The Request");

    public static Error NoDriverAvailable =>
        Error.Validation("No_Driver_Available", "No Driver Is Available Right Now");

    // ================================================================
    // Reservation Rules
    // ================================================================

    public static Error RestaurantNotAcceptingReservations =>
        Error.Validation("Restaurant_Not_Accepting_Reservations", "Restaurant Is Not Accepting Reservations");

    public static Error ReservationTimeInPast =>
        Error.Validation("Reservation_Time_In_Past", "Reservation Time Cannot Be In The Past");

    public static Error ReservationTimeOutsideHours =>
        Error.Validation("Reservation_Time_Outside_Hours", "Reservation Time Is Outside Restaurant Hours");

    public static Error ReservationPartySizeExceeded =>
        Error.Validation("Reservation_Party_Size_Exceeded", "Party Size Exceeds The Maximum Allowed");

    public static Error ReservationSlotFull =>
        Error.Validation("Reservation_Slot_Full", "This Time Slot Is Fully Booked");

    public static Error ReservationAlreadyCancelled =>
        Error.Validation("Reservation_Already_Cancelled", "Reservation Has Already Been Cancelled");

    public static Error ReservationNoShow =>
        Error.Validation("Reservation_No_Show", "Customer Did Not Show Up For The Reservation");

    // ================================================================
    // Discount Rules
    // ================================================================

    public static Error DiscountExpired =>
        Error.Validation("Discount_Expired", "Discount Code Has Expired");

    public static Error DiscountNotStarted =>
        Error.Validation("Discount_Not_Started", "Discount Code Is Not Active Yet");

    public static Error DiscountAlreadyUsed =>
        Error.Validation("Discount_Already_Used", "Discount Code Has Already Been Used");

    public static Error DiscountUsageLimitReached =>
        Error.Validation("Discount_Usage_Limit_Reached", "Discount Usage Limit Has Been Reached");

    public static Error DiscountNotApplicableToItems =>
        Error.Validation("Discount_Not_Applicable_To_Items", "Discount Is Not Applicable To The Selected Items");

    public static Error DiscountBelowMinimumOrder =>
        Error.Validation("Discount_Below_Minimum_Order", "Order Total Does Not Meet The Discount Minimum");

    // ================================================================
    // Payment Rules
    // ================================================================

    public static Error PaymentAlreadyCompleted =>
        Error.Validation("Payment_Already_Completed", "Payment Has Already Been Completed");

    public static Error PaymentFailed =>
        Error.Validation("Payment_Failed", "Payment Could Not Be Processed");

    public static Error PaymentMethodNotSupported =>
        Error.Validation("Payment_Method_Not_Supported", "Selected Payment Method Is Not Supported");

    public static Error InsufficientFunds =>
        Error.Validation("Insufficient_Funds", "Insufficient Funds");

    public static Error RefundNotAllowed =>
        Error.Validation("Refund_Not_Allowed", "Refund Is Not Allowed At This Stage");

    // ================================================================
    // Review Rules
    // ================================================================

    public static Error ReviewAlreadySubmitted =>
        Error.Validation("Review_Already_Submitted", "Review Has Already Been Submitted For This Order");

    public static Error ReviewNotAllowed =>
        Error.Validation("Review_Not_Allowed", "Cannot Submit A Review At This Stage");

    public static Error ReviewTargetInvalid =>
        Error.Validation("Review_Target_Invalid", "Review Must Target Either A Restaurant Or A Driver");

    // ================================================================
    // Complaint Rules
    // ================================================================

    public static Error ComplaintAlreadyClosed =>
        Error.Validation("Complaint_Already_Closed", "Complaint Is Already Closed");

    public static Error ComplaintCannotBeReopened =>
        Error.Validation("Complaint_Cannot_Be_Reopened", "Complaint Cannot Be Reopened");

    public static Error ComplaintMessageNotAllowed =>
        Error.Validation("Complaint_Message_Not_Allowed", "Cannot Add A Message To A Closed Complaint");

    // ================================================================
    // Service Zone Rules
    // ================================================================

    public static Error ServiceZoneInactive =>
        Error.Validation("Service_Zone_Inactive", "Service Zone Is Currently Inactive");

    public static Error AddressOutsideServiceZone =>
        Error.Validation("Address_Outside_Service_Zone", "Address Is Outside Any Active Service Zone");

    // ================================================================
    // General Rules
    // ================================================================

    public static Error OperationNotAllowedInCurrentState =>
        Error.Validation("Operation_Not_Allowed_In_Current_State", "Operation Is Not Allowed In The Current State");

    public static Error ConcurrentModification =>
        Error.Validation("Concurrent_Modification", "The Record Has Been Modified By Another User");

    public static Error RateLimitExceeded =>
        Error.Validation("Rate_Limit_Exceeded", "Too Many Requests. Please Try Again Later");

    public static Error DailyLimitExceeded =>
        Error.Validation("Daily_Limit_Exceeded", "Daily Limit Has Been Exceeded");

    public static Error CategoryNotFound =>
        Error.NotFound("Category_NotFound", "Category Not Found");

    public static Error AddOnNotFound =>
        Error.NotFound("AddOn_NotFound", "AddOn Not Found");

    public static Error PrinterNotFound =>
    Error.NotFound("Printer_Not_Found", "Printer Not Found");

    // ================================================================
    // User & Auth Rules
    // ================================================================

    public static Error EmailRequired =>
        Error.Validation("Email_Required", "Email Is Required");

    public static Error EmailInvalid =>
        Error.Validation("Email_Invalid", "Email Format Is Invalid");

    public static Error EmailAlreadyExists =>
        Error.Conflict("Email_Already_Exists", "An Account With This Email Already Exists");

    public static Error PhoneRequired =>
        Error.Validation("Phone_Required", "Phone Number Is Required");

    public static Error PhoneInvalid =>
        Error.Validation("Phone_Invalid", "Phone Number Format Is Invalid");

    public static Error PhoneAlreadyExists =>
        Error.Conflict("Phone_Already_Exists", "An Account With This Phone Number Already Exists");

    public static Error PasswordRequired =>
        Error.Validation("Password_Required", "Password Is Required");

    public static Error PasswordTooWeak =>
        Error.Validation("Password_Too_Weak", "Password Does Not Meet Security Requirements");

    public static Error NameArRequired =>
        Error.Validation("Name_Ar_Required", "Arabic Name Is Required");

    public static Error NameEnRequired =>
        Error.Validation("Name_En_Required", "English Name Is Required");

    public static Error RoleNotFound =>
        Error.Validation("Role_Not_Found", "The Specified Role Does Not Exist");

    public static Error RoleAssignmentFailed =>
        Error.Failure("Role_Assignment_Failed", "Failed To Assign Role To User");

    public static Error UserCreationFailed =>
        Error.Failure("User_Creation_Failed", "Failed To Create User Account");

    public static Error InvalidCredentials =>
        Error.Unauthorized("Invalid_Credentials", "Email Or Password Is Incorrect");

    public static Error UserNotFound =>
        Error.NotFound("User_Not_Found", "User Account Not Found");

    public static Error AccountLocked =>
        Error.Forbidden("Account_Locked", "Account Is Locked");

    public static Error AccountSuspended =>
        Error.Forbidden("Account_Suspended", "Account Has Been Suspended");

    public static Error ExpiredAccessTokenInvalid =>
        Error.Validation("Expired_Access_Token_Invalid", "The Expired Access Token Is Not Valid");

    public static Error UserIdClaimInvalid =>
        Error.Validation("User_Id_Claim_Invalid", "The User Id Claim Is Missing Or Invalid");

    public static Error RefreshTokenExpired =>
        Error.Validation("Refresh_Token_Expired", "The Refresh Token Has Expired Or Is Invalid");

    public static Error AccessDenied =>
        Error.Validation("Access_Denied", "Access to this page is not allowed.");

    public static Error Unauthorized =>
        Error.Unauthorized("Unauthorized", "Unauthorized User");
}