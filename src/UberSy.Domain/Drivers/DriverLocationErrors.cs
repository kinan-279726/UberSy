using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Drivers;

public static class DriverLocationErrors
{
    public static Error BoundaryInvalid =>
        Error.Validation("Location_Invalid", "Invalid points");

    public static Error BoundaryRequired =>
        Error.Validation("Location_Point_Required", "Points is required");
}