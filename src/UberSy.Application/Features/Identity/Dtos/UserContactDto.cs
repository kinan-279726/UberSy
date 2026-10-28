namespace UberSy.Application.Features.Identity.Dtos;

public sealed class UserContactDto
{
    public Guid UserId { get; init; }
    public string NameAr { get; init; } = null!;
    public string NameEn { get; init; } = null!;
    public string? Phone { get; init; }
}