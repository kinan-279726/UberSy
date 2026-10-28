using System.ComponentModel;

namespace UberSy.Domain.ServiceRequests.Enums
{
    public enum  ServiceType : byte
    {
        [Description("Food")]
        Food = 1,

        [Description("Reservation")]
        Reservation = 2,

        [Description("Ride")]
        Ride = 3
    }
}
