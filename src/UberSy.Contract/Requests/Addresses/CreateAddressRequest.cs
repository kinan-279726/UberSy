namespace UberSy.Contract.Requests.Addresses;

public sealed record CreateAddressRequest(
    string Street, string City, double Latitude, double Longitude,
    string? Label, string? Floor, string? Phone);

public sealed record UpdateAddressRequest(
    string Street, string City, double Latitude, double Longitude,
    string? Label, string? Floor, string? Phone);