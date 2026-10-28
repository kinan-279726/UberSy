using Microsoft.AspNetCore.Identity;

namespace UberSy.Infrastructure.Identity;

public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() : base()
    {
        Id = Guid.NewGuid();
    }

    public ApplicationRole(string roleName) : base(roleName)
    {
        Id = Guid.NewGuid();
    }
}
