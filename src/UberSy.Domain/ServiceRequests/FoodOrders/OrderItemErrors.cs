using UberSy.Domain.Common.Results;

namespace UberSy.Domain.ServiceRequests.FoodOrders;

public static class OrderItemErrors
{
    public static Error ItemIdRequired =>
        Error.Validation("Item_Id_Required", "Item Id Is Required");

    public static Error QuantityZero =>
        Error.Validation("Quantity_Zero", "Quantity Must Be Greater Than Zero");

    public static Error UnitPriceZero =>
        Error.Validation("Unit_Price_Zero", "Unit Price Must Be Greater Than Zero");

    public static Error NoteTooLong =>
        Error.Validation("Note_Too_Long", "Note Cannot Exceed 100 Characters");

    public static Error AddOnPriceNegative =>
        Error.Validation("AddOn_Price_Negative", "Add-On Price Cannot Be Negative");
}
