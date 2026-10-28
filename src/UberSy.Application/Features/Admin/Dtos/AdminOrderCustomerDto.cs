namespace UberSy.Application.Features.Admin.Dtos;

public sealed class AdminOrderCustomerDto
{
    public Guid UserId { get; init; }
    public string? Email { get; init; }
    public string AddressLabel { get; init; } = null!;
    public string Street { get; init; } = null!;
    public string City { get; init; } = null!;
    public string? Floor { get; init; }
    public string? Phone { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}