using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Discounts;

public static class DiscountErrors
{
    public static Error CustomerIdRequired =>
        Error.Validation("Customer_Id_Required", "Customer Id Is Required");

    public static Error DescountrIdRequired =>
        Error.Validation("Descount_Id_Required", "Descount Id Is Required");

    public static Error ServiceIdRequired =>
        Error.Validation("Service_Id_Required", "Service Id Is Required");

    public static Error CodeTooLong =>
        Error.Validation("Code_Too_Long", "Code Cannot Exceed 20 Characters");

    public static Error ValueZero =>
        Error.Validation("Value_Zero", "Value Must Be Greater Than Zero");

    public static Error PercentageExceeds100 =>
        Error.Validation("Percentage_Exceeds_100", "Percentage Discount Cannot Exceed 100");

    public static Error MinOrderAmountNegative =>
        Error.Validation("Min_Order_Amount_Negative", "Minimum Order Amount Cannot Be Negative");

    public static Error MaxUsageCountZero =>
        Error.Validation("Max_Usage_Count_Zero", "Max Usage Count Must Be Greater Than Zero");

    public static Error EndDateBeforeStartDate =>
        Error.Validation("End_Date_Before_Start_Date", "End Date Must Be After Start Date");

    public static Error CodeAlreadyExists =>
        Error.Conflict("Code_Already_Exists", "A Discount With This Code Already Exists");

    public static Error DiscountAlreadyActive =>
        Error.Conflict("Discount_Already_Active", "Discount Is Already Active");

    public static Error DiscountAlreadyInactive =>
        Error.Conflict("Discount_Already_Inactive", "Discount Is Already Inactive");
}