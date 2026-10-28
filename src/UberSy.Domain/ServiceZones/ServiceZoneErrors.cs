using UberSy.Domain.Common.Results;

namespace UberSy.Domain.ServiceZones;

public static class ServiceZoneErrors
{
    // ============ NameAr ============
    public static Error NameArRequired =>
        Error.Validation("Name_Ar_Required", "Arabic Name Is Required");

    public static Error NameArTooLong =>
        Error.Validation("Name_Ar_Too_Long", "Arabic Name Cannot Exceed 100 Characters");

    public static Error NameArAlreadyExists =>
        Error.Conflict("Name_Ar_Already_Exists", "A Service Zone With This Arabic Name Already Exists");

    // ============ NameEn ============
    public static Error NameEnRequired =>
        Error.Validation("Name_En_Required", "English Name Is Required");

    public static Error NameEnTooLong =>
        Error.Validation("Name_En_Too_Long", "English Name Cannot Exceed 100 Characters");

    public static Error NameEnAlreadyExists =>
        Error.Conflict("Name_En_Already_Exists", "A Service Zone With This English Name Already Exists");

    // ============ Boundary ============
    public static Error BoundaryRequired =>
        Error.Validation("Boundary_Required", "Boundary Is Required");

    public static Error BoundaryInvalid =>
        Error.Validation("Boundary_Invalid", "Boundary Must Be A Valid Geographic Shape");

    public static Error BoundaryMustBePolygon =>
        Error.Validation("Boundary_Must_Be_Polygon", "Boundary Must Be A Polygon Or MultiPolygon");

    public static Error BoundaryTooLarge =>
        Error.Validation("Boundary_Too_Large", "Boundary Area Exceeds The Maximum Allowed Size");

    public static Error BoundaryOverlapsExisting =>
        Error.Conflict("Boundary_Overlaps_Existing", "Boundary Overlaps With An Existing Active Service Zone");

    public static Error BoundaryInvalidCoordinates =>
        Error.Validation("Boundary_Invalid_Coordinates", "Boundary Contains Invalid Coordinates");

    public static Error BoundaryNotClosed =>
    Error.Validation("Boundary_Not_Closed", "Polygon Boundary Must Be Closed (First Point = Last Point)");

    public static Error BoundarySelfIntersects =>
    Error.Validation("Boundary_Self_Intersects", "Polygon Boundary Cannot Self-Intersect");

    // ============ City ============
    public static Error CityRequired =>
        Error.Validation("City_Required", "City Is Required");

    public static Error CityTooLong =>
        Error.Validation("City_Too_Long", "City Name Cannot Exceed 100 Characters");

    public static Error CityInvalid =>
        Error.Validation("City_Invalid", "City Is Not Valid");

    // ============ IsActive ============
    public static Error IsActiveRequired =>
        Error.Validation("Is_Active_Required", "Is Active Is Required");

    // ============ General ============
    public static Error ServiceZoneNotFound =>
        Error.NotFound("Service_Zone_Not_Found", "Service Zone Not Found");

    public static Error ServiceZoneAlreadyExists =>
        Error.Conflict("Service_Zone_Already_Exists", "A Service Zone With The Same Name Already Exists");

    public static Error ServiceZoneInUse =>
        Error.Conflict("Service_Zone_In_Use", "Service Zone Cannot Be Deleted Because It Is In Use");

    public static Error ServiceZoneHasActiveDrivers =>
        Error.Conflict("Service_Zone_Has_Active_Drivers", "Service Zone Cannot Be Deactivated While It Has Active Drivers");

    public static Error ServiceZoneHasActiveOrders =>
        Error.Conflict("Service_Zone_Has_Active_Orders", "Service Zone Cannot Be Deactivated While It Has Active Orders");
}
