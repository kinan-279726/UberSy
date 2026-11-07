using NSubstitute;
using UberSy.Domain.Identity;
using UberSy.Infrastructure.Identity;

namespace UberSy.Test.Common.Security;

internal class UserFactory
{
    public static ApplicationUser CreateUser()
    {
        return new ApplicationUser
        {
            Id = Guid.Parse("12345678-1234-1234-1234-000012345678"),
            Email = "test@gmail.com",
            UserName = "test@gmail.com",
            EmailConfirmed = true,
            PhoneNumber = "+963912345678",
            PhoneNumberConfirmed = true
        };
    }
}
