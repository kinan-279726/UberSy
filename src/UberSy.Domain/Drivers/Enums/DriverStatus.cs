using System.ComponentModel;

namespace UberSy.Domain.Drivers.Enums;

public enum DriverStatus : byte
{
    [Description("Offline")]
    Offline = 1,

    [Description("Online")]
    Online = 2,

    [Description("OnDelivery")]
    OnDelivery = 3,

    [Description("Stopped")]
    Stopped = 4,
}