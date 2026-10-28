namespace UberSy.Application.Features.Identity.Dtos;

public sealed class AdminUserListItemDto
{
    public Guid Id { get; init; }
    public string? Email { get; init; }
    public string NameAr { get; init; } = null!;
    public string NameEn { get; init; } = null!;
    public string? Phone { get; init; }
    public bool EmailConfirmed { get; init; }
    public bool PhoneConfirmed { get; init; }
    public DateTime? BannedUntil { get; init; }
    public List<string> Roles { get; init; } = [];
}