using UberSy.Application.Common.Interfaces;
using UberSy.Infrastructure.Identity;

namespace UberSy.Test.Common.Security;

public class TestCurrentUser : IUser
{
    private ApplicationUser? _currentUser;
    private readonly List<string> _roles = [];

    public void Returns(ApplicationUser currentUser, params string[] roles)
    {
        _currentUser = currentUser;
        _roles.Clear();
        _roles.AddRange(roles);
    }

    public bool IsInRole(string roleName)
    {
        if (_roles.Any(r => string.Equals(r, roleName, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return false;
    }

    public string? Id => _currentUser?.Id.ToString();

    public void Reset()
    {
        _currentUser = null;
        _roles.Clear();
    }

}
