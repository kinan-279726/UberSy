namespace UberSy.Contract.Requests.Drivers;

public sealed record RegisterDriverRequest(
    byte VehicleType, byte? CarType, string NationalId, string? PhotoUrl,
    string? LicenseNumber, string? PlateNumber, string? Model, string? Color);
