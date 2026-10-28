using System.ComponentModel;

namespace UberSy.Domain.ServiceRequests.Enums
{
    public enum CancellationReason : byte
    {
        [Description("CustomerChangedMind")]
        CustomerChangedMind = 1,

        [Description("CustomerNotFound")]
        CustomerNotFound = 2,

        [Description("RestaurantClosed")]
        RestaurantClosed = 3,

        [Description("RestaurantOutOfStock")]
        RestaurantOutOfStock = 4,

        [Description("NoDriverAvailable")]
        NoDriverAvailable = 5,

        [Description("DriverTooFar")]
        DriverTooFar = 6,

        [Description("PaymentFailed")]
        PaymentFailed = 7,

        [Description("DeliveryTooLong")]
        DeliveryTooLong = 8,

        [Description("DuplicateRequest")]
        DuplicateRequest = 9,

        [Description("Other")]
        Other = 99
    }
}
