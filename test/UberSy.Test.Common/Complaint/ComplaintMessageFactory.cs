using UberSy.Domain.Common.Results;
using UberSy.Domain.Complaints;

namespace UberSy.Test.Common.Complaint;

public static class ComplaintMessageFactory
{
    public static Result<TbComplaintMessage> CreateComplaintMessage(
        Guid? id = null,
        Guid? complaintId = null,
        Guid? senderUserId = null,
        string? message = null)
    {
        return TbComplaintMessage.Create(
            id ?? Guid.NewGuid(),
            complaintId ?? Guid.NewGuid(),
            senderUserId ?? Guid.NewGuid(),
            message ?? "Any update on my complaint?");
    }
}
