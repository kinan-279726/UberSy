using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Reservations;

public static class TableReservationErrors
{
    public static Error RestaurantIdRequired =>
        Error.Validation("Restaurant_Id_Required", "Restaurant Id Is Required");

    public static Error ReservationTimeMustBeFuture =>
        Error.Validation("Reservation_Time_Must_Be_Future", "Reservation Time Must Be In The Future");

    public static Error PartySizeZero =>
        Error.Validation("Party_Size_Zero", "Party Size Must Be Greater Than Zero");

    public static Error DepositAmountNegative =>
        Error.Validation("Deposit_Amount_Negative", "Deposit Amount Cannot Be Negative");

    public static Error NoteTooLong =>
        Error.Validation("Note_Too_Long", "Note Cannot Exceed 1000 Characters");
}
