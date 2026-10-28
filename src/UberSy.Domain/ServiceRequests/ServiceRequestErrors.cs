using UberSy.Domain.Common.Results;

namespace UberSy.Domain.ServiceRequests;

public static class ServiceRequestErrors
{
    // ================================================================
    // Validation
    // ================================================================

    // ============ CustomerId ============
    public static Error CustomerIdRequired =>
        Error.Validation("Customer_Id_Required", "Customer Id Is Required");

    public static Error CustomerIdInvalid =>
        Error.Validation("Customer_Id_Invalid", "Customer Id Is Invalid");

    // ============ ServiceType ============
    public static Error ServiceTypeRequired =>
        Error.Validation("Service_Type_Required", "Service Type Is Required");

    public static Error ServiceTypeInvalid =>
        Error.Validation("Service_Type_Invalid", "Service Type Must Be 1 (Food), 2 (Reservation), or 3 (Ride)");

    // ============ TrackingNumber ============
    public static Error TrackingNumberRequired =>
        Error.Validation("Tracking_Number_Required", "Tracking Number Is Required");

    public static Error TrackingNumberInvalid =>
        Error.Validation("Tracking_Number_Invalid", "Tracking Number Is Invalid");

    public static Error TrackingNumberNegative =>
        Error.Validation("Tracking_Number_Negative", "Tracking Number Cannot Be Negative");

    // ============ Status ============
    public static Error StatusRequired =>
        Error.Validation("Status_Required", "Status Is Required");

    public static Error StatusInvalid =>
        Error.Validation("Status_Invalid", "Status Must Be 1 (Pending), 2 (Preparing), 3 (Out For Delivery), 4 (Delivered), or 5 (Cancelled)");

    // ============ TotalAmount ============
    public static Error TotalAmountRequired =>
        Error.Validation("Total_Amount_Required", "Total Amount Is Required");

    public static Error TotalAmountNegative =>
        Error.Validation("Total_Amount_Negative", "Total Amount Cannot Be Negative");

    public static Error TotalAmountZero =>
        Error.Validation("Total_Amount_Zero", "Total Amount Must Be Greater Than Zero");

    public static Error TotalAmountTooLarge =>
        Error.Validation("Total_Amount_Too_Large", "Total Amount Exceeds The Maximum Allowed Value");

    // ============ CancellationReason ============
    public static Error CancellationReasonRequired =>
        Error.Validation("Cancellation_Reason_Required", "Cancellation Reason Is Required When Status Is Cancelled");

    public static Error CancellationReasonTooLong =>
        Error.Validation("Cancellation_Reason_Too_Long", "Cancellation Reason Cannot Exceed 100 Characters");

    public static Error CancellationReasonNotAllowed =>
        Error.Validation("Cancellation_Reason_Not_Allowed", "Cancellation Reason Is Only Allowed When Status Is Cancelled");

    // ================================================================
    // Conflict
    // ================================================================

    public static Error ServiceRequestAlreadyCancelled =>
        Error.Conflict("Service_Request_Already_Cancelled", "Service Request Is Already Cancelled");

    public static Error ServiceRequestAlreadyDelivered =>
        Error.Conflict("Service_Request_Already_Delivered", "Service Request Is Already Delivered");

    public static Error ServiceRequestAlreadyCompleted =>
        Error.Conflict("Service_Request_Already_Completed", "Service Request Is Already Completed");

    public static Error CannotCancelDeliveredRequest =>
        Error.Conflict("Cannot_Cancel_Delivered_Request", "Cannot Cancel A Delivered Service Request");

    public static Error CannotCancelOutForDeliveryRequest =>
        Error.Conflict("Cannot_Cancel_Out_For_Delivery_Request", "Cannot Cancel A Service Request That Is Out For Delivery");

    public static Error CannotModifyCompletedRequest =>
        Error.Conflict("Cannot_Modify_Completed_Request", "Cannot Modify A Completed Service Request");

    public static Error InvalidStatusTransition =>
        Error.Conflict("Invalid_Status_Transition", "Invalid Status Transition For This Service Request");

    public static Error ServiceRequestInUse =>
        Error.Conflict("Service_Request_In_Use", "Service Request Cannot Be Deleted Because It Is In Use");

    public static Error ServiceRequestHasPayments =>
        Error.Conflict("Service_Request_Has_Payments", "Service Request Cannot Be Deleted While It Has Payments");

    public static Error ServiceRequestHasReviews =>
        Error.Conflict("Service_Request_Has_Reviews", "Service Request Cannot Be Deleted While It Has Reviews");

    public static Error ServiceRequestHasComplaints =>
        Error.Conflict("Service_Request_Has_Complaints", "Service Request Cannot Be Deleted While It Has Complaints");

    // ================================================================
    // NotFound
    // ================================================================

    public static Error ServiceRequestNotFound =>
        Error.NotFound("Service_Request_Not_Found", "Service Request Not Found");

    public static Error CustomerNotFound =>
        Error.NotFound("Customer_Not_Found", "Customer Not Found");

    // ================================================================
    // Forbidden
    // ================================================================

    public static Error NotServiceRequestOwner =>
        Error.Forbidden("Not_Service_Request_Owner", "You Do Not Own This Service Request");

    public static Error CustomerSuspended =>
        Error.Forbidden("Customer_Suspended", "Customer Account Has Been Suspended");

    public static Error CustomerBlocked =>
        Error.Forbidden("Customer_Blocked", "Customer Account Has Been Blocked");

    public static Error CannotCancelInCurrentStatus =>
        Error.Forbidden("Cannot_Cancel_In_Current_Status", "Cannot Cancel Service Request In Its Current Status");

    public static Error CannotModifyInCurrentStatus =>
        Error.Forbidden("Cannot_Modify_In_Current_Status", "Cannot Modify Service Request In Its Current Status");

    public static Error ServiceUnavailable =>
        Error.Forbidden("Service_Unavailable", "This Service Is Currently Unavailable");

    public static Error ServiceNotAvailableInArea =>
        Error.Forbidden("Service_Not_Available_In_Area", "This Service Is Not Available In Your Area");

    // ================================================================
    // Failure
    // ================================================================

    public static Error ServiceRequestCreationFailed =>
        Error.Failure("Service_Request_Creation_Failed", "Failed To Create Service Request");

    public static Error ServiceRequestUpdateFailed =>
        Error.Failure("Service_Request_Update_Failed", "Failed To Update Service Request");

    public static Error ServiceRequestCancellationFailed =>
        Error.Failure("Service_Request_Cancellation_Failed", "Failed To Cancel Service Request");

    public static Error ServiceRequestDeletionFailed =>
        Error.Failure("Service_Request_Deletion_Failed", "Failed To Delete Service Request");

    public static Error NoDriverAvailable =>
        Error.Failure("No_Driver_Available", "No Driver Is Currently Available For This Request");

    public static Error NoRestaurantAvailable =>
        Error.Failure("No_Restaurant_Available", "No Restaurant Is Currently Available For This Request");

    public static Error StatusUpdateFailed =>
        Error.Failure("Status_Update_Failed", "Failed To Update Service Request Status");

    // ================================================================
    // Unexpected
    // ================================================================

    public static Error UnexpectedError =>
        Error.Unexpected("Unexpected_Error", "An Unexpected Error Occurred While Processing Service Request");
}