using UberSy.Domain.ServiceRequests.History;

namespace UberSy.Test.Common.ServiceRequest;

public static class ServiceRequestHistoryFactory
{
    public static TbServiceRequestHistory CreateServiceRequestHistory(
        Guid? serviceRequestId = null,
        byte? status = null,
        string? note = null)
    {
        // لا توجد دالة Create — إنشاء مباشر
        return new TbServiceRequestHistory
        {
            ServiceRequestId = serviceRequestId ?? Guid.NewGuid(),
            Status = status ?? 1, // Pending
            Note = note,
        };
    }
}
