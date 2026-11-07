using UberSy.Domain.Common.Results;
using UberSy.Domain.Complaints;

namespace UberSy.Test.Common.Complaint;

public static class ComplaintFactory
{
    public static Result<TbComplaint> CreateComplaint(
        Guid? id = null,
        Guid? userId = null,
        Guid? serviceRequestId = null,
        string? subject = null,
        string? message = null)
    {
        return TbComplaint.Create(
            id ?? Guid.NewGuid(),
            userId ?? Guid.NewGuid(),
            serviceRequestId,
            subject ?? "Late Delivery",
            message ?? "My order arrived 40 minutes late.");
    }
}
