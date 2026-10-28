using UberSy.Domain.Common;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Complaints.Enums;
using UberSy.Domain.ServiceRequests;

namespace UberSy.Domain.Complaints;

public sealed class TbComplaint : AuditableEntity
{
    public Guid UserId { get; private set; }
    public Guid? ServiceRequestId { get; private set; }
    public string Subject { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public ComplaintStatus Status { get; private set; }

    public TbServiceRequest? ServiceRequest { get; private set; }

    private readonly List<TbComplaintMessage> _messages = [];
    public IReadOnlyCollection<TbComplaintMessage> Messages => _messages.AsReadOnly();

    private TbComplaint()
    { }

    private TbComplaint(Guid id, Guid userId, Guid? serviceRequestId, string subject, string message)
        : base(id)
    {
        UserId = userId;
        ServiceRequestId = serviceRequestId;
        Subject = subject;
        Message = message;
        Status = ComplaintStatus.Open;
    }

    public static Result<TbComplaint> Create(Guid id, Guid userId, Guid? serviceRequestId, string subject, string message)
    {
        if (userId == Guid.Empty) return ComplaintErrors.UserIdRequired;

        if (string.IsNullOrWhiteSpace(subject)) return ComplaintErrors.SubjectRequired;
        if (subject.Length > 200) return ComplaintErrors.SubjectTooLong;

        if (string.IsNullOrWhiteSpace(message)) return ComplaintErrors.MessageRequired;
        if (message.Length > 2000) return ComplaintErrors.MessageTooLong;

        return new TbComplaint(id, userId, serviceRequestId, subject.Trim(), message.Trim());
    }

    public Result<Updated> MarkInProgress()
    {
        if (Status == ComplaintStatus.Closed) return ComplaintErrors.ComplaintAlreadyClosed;

        Status = ComplaintStatus.InProgress;

        return Result.Updated;
    }

    public Result<Updated> Resolve()
    {
        if (Status == ComplaintStatus.Closed) return ComplaintErrors.ComplaintAlreadyClosed;

        Status = ComplaintStatus.Resolved;

        return Result.Updated;
    }

    public Result<Updated> Close()
    {
        if (Status == ComplaintStatus.Closed) return ComplaintErrors.ComplaintAlreadyClosed;

        Status = ComplaintStatus.Closed;

        return Result.Updated;
    }

    public Result<Updated> Reopen()
    {
        Status = ComplaintStatus.Open;

        return Result.Updated;
    }
}