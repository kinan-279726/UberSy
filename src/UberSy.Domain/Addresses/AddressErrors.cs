using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Addresses;

public static class AddressErrors
{
    public static Error UserIdRequired =>
        Error.Validation("User_Id_Required", "User Id Is Required");

    public static Error StreetRequired =>
        Error.Validation("Street_Required", "Street Is Required");

    public static Error StreetTooLong =>
        Error.Validation("Street_Too_Long", "Street Cannot Exceed 100 Characters");

    public static Error CityRequired =>
        Error.Validation("City_Required", "City Is Required");

    public static Error CityTooLong =>
        Error.Validation("City_Too_Long", "City Cannot Exceed 100 Characters");

    public static readonly Error InvalidPhoneNumber =
        Error.Conflict("Customer.InvalidPhoneNumber", "Phone number must start with +9639 and end with 8 digits.");

    public static Error BoundaryRequired =>
        Error.Validation("Boundary_Required", "Address Location Is Required");

    public static Error BoundaryInvalid =>
        Error.Validation("Boundary_Invalid", "Address Location Coordinates Are Invalid");

    public static Error BoundaryOutsideServiceZone =>
        Error.Validation("Boundary_Outside_Service_Zone", "Address Is Outside Any Active Service Zone");

    public static Error LabelTooLong =>
        Error.Validation("Label_Too_Long", "Label Cannot Exceed 100 Characters");

    public static Error PhoneTooLong =>
        Error.Validation("Phone_Too_Long", "Phone Cannot Exceed 20 Characters");
}
 