using System.Security.Claims;

namespace UberSy.Application.Features.Identity;

public sealed record ApplicationUserDto(string UserId, string Email, IList<string>? Roles, IList<string>? Permission, IList<Claim>? Claims);
public sealed record ApplicationUserGuidDto(Guid UserId, string? Email, IList<string>? Roles, IList<string>? Permission, IList<Claim>? Claims);
