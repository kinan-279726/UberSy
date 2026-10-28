using System.ComponentModel;

namespace UberSy.Domain.ServiceRequests.Enums
{
    public enum ServiceRequestStatus : byte
    {
        [Description("Pending")]
        Pending = 1,

        [Description("Preparing")]
        Preparing = 2,

        [Description("OutForDelivery")]
        OutForDelivery = 3,

        [Description("Delivered")]
        Delivered = 4,

        [Description("Cancelled")]
        Cancelled = 5
    }
}
