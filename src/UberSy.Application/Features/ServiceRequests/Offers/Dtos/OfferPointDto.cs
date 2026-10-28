namespace UberSy.Application.Features.ServiceRequests.Offers.Dtos;

public sealed class OfferPointDto
{
    public string NameAr { get; init; } = null!;
    public string? NameEn { get; init; }
    public string? Street { get; init; }
    public string? City { get; init; }
    public string? Floor { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
}
