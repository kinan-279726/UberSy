using System.Security.Claims;
using UberSy.Application.Common.Interfaces;

namespace UberSy.Api.Services;

public class CurrentUser : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public bool IsInRole(string roleName)
    {
        var result = _httpContextAccessor.HttpContext?.User.IsInRole(roleName);
        if (result is null) return false;

        return result.Value;
    }
}
