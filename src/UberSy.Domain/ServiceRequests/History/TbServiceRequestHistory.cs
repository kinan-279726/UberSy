using UberSy.Domain.Common;

namespace UberSy.Domain.ServiceRequests.History;

public partial class TbServiceRequestHistory :AuditableEntity
{
    public Guid ServiceRequestId { get; set; }

    public byte Status { get; set; }

    public string? Note { get; set; }

    public virtual TbServiceRequest ServiceRequest { get; set; } = null!;
}
