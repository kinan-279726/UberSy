using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Payments;

public static class PaymentErrors
{
    public static Error ServiceRequestIdRequired =>
        Error.Validation("Service_Request_Id_Required", "Service Request Id Is Required");

    public static Error AmountZero =>
        Error.Validation("Amount_Zero", "Amount Must Be Greater Than Zero");

    public static Error TransactionIdTooLong =>
        Error.Validation("Transaction_Id_Too_Long", "Transaction Id Cannot Exceed 100 Characters");

    public static Error PaymentAlreadyCompleted =>
        Error.Conflict("Payment_Already_Completed", "Payment Is Already Marked As Completed");

    public static Error PaymentAlreadyFailed =>
        Error.Conflict("Payment_Already_Failed", "Payment Is Already Marked As Failed");
}