using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Complaints;

public sealed class TbComplaintMessage : AuditableEntity
{
    public Guid ComplaintId { get; private set; }
    public Guid SenderUserId { get; private set; }
    public string Message { get; private set; } = null!;

    public TbComplaint Complaint { get; private set; } = null!;

    private TbComplaintMessage()
    { }

    private TbComplaintMessage(Guid id, Guid complaintId, Guid senderUserId, string message)
        : base(id)
    {
        ComplaintId = complaintId;
        SenderUserId = senderUserId;
        Message = message;
    }

    public static Result<TbComplaintMessage> Create(Guid id, Guid complaintId, Guid senderUserId, string message)
    {
        if (senderUserId == Guid.Empty) return ComplaintErrors.UserIdRequired;
        if (string.IsNullOrWhiteSpace(message)) return ComplaintErrors.MessageRequired;
        if (message.Length > 2000) return ComplaintErrors.MessageTooLong;

        return new TbComplaintMessage(id, complaintId, senderUserId, message.Trim());
    }
}
