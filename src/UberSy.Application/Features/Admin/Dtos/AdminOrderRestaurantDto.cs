namespace UberSy.Application.Features.Admin.Dtos;

public sealed class AdminOrderRestaurantDto
{
    public Guid Id { get; init; }
    public string NameAr { get; init; } = null!;
    public string? Phone { get; init; }
}
