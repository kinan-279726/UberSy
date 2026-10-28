using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Reviews;

public static class ReviewErrors
{
    public static Error CustomerIdRequired =>
        Error.Validation("Customer_Id_Required", "Customer Id Is Required");

    public static Error ServiceRequestIdRequired =>
        Error.Validation("Service_Request_Id_Required", "Service Request Id Is Required");

    public static Error ExactlyOneTargetRequired =>
        Error.Validation("Exactly_One_Target_Required", "Review Must Target Either A Restaurant Or A Driver, Not Both Or Neither");

    public static Error RatingOutOfRange =>
        Error.Validation("Rating_Out_Of_Range", "Rating Must Be Between 1 And 5");

    public static Error NoteTooLong =>
        Error.Validation("Note_Too_Long", "Note Cannot Exceed 1000 Characters");
}