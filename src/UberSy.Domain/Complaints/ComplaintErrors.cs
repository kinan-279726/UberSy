using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Complaints;

public static class ComplaintErrors
{
    public static Error UserIdRequired =>
        Error.Validation("User_Id_Required", "User Id Is Required");

    public static Error SubjectRequired =>
        Error.Validation("Subject_Required", "Subject Is Required");

    public static Error SubjectTooLong =>
        Error.Validation("Subject_Too_Long", "Subject Cannot Exceed 200 Characters");

    public static Error MessageRequired =>
        Error.Validation("Message_Required", "Message Is Required");

    public static Error MessageTooLong =>
        Error.Validation("Message_Too_Long", "Message Cannot Exceed 2000 Characters");

    public static Error ComplaintAlreadyClosed =>
        Error.Conflict("Complaint_Already_Closed", "Complaint Is Already Closed");

    public static Error CannotMessageClosedComplaint =>
        Error.Forbidden("Cannot_Message_Closed_Complaint", "Cannot Send Messages On A Closed Complaint");
}