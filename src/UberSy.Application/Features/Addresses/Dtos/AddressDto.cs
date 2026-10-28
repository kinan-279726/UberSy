namespace UberSy.Application.Features.Addresses.Dtos;
public sealed class AddressDto
{
    public Guid Id { get; init; }
    
    public Guid UserId { get; init; }
    
    public string Street { get; init; } = null!;
    
    public string City { get; init; } = null!;
    
    public double Latitude { get; init; }
    
    public double Longitude { get; init; }
    
    public string? Label { get; init; }
    
    public string? Floor { get; init; }
    
    public string? Phone { get; init; }
    
    public bool IsDefault { get; init; }
}
