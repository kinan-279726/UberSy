using UberSy.Domain.Restaurants.Printers;

namespace UberSy.Test.Common.Printer;

public static class PrinterFactory
{
    public static TbPrinter CreatePrinter(
        Guid? id = null,
        Guid? restaurantId = null,
        byte? type = null,
        string? name = null,
        string? connectionInfo = null,
        bool? isActive = null)
    {
        var printer = new TbPrinter
        {
            RestaurantId = restaurantId ?? Guid.NewGuid(),
            Type = type ?? 1,
            Name = name ?? "Kitchen Printer",
            ConnectionInfo = connectionInfo ?? "192.168.1.100:9100",
            IsActive = isActive ?? true,
        };

        return printer;
    }
}
