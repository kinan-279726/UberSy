using UberSy.Domain.Common.Constants;
using UberSy.Infrastructure.Identity;

namespace UberSy.Test.Common.Security;

public static class TestUsers
{
    // 1. Admin
    public static ApplicationUser Admin => new()
    {
        Id = Guid.Parse(SystemConstants.ManagerId),
        NameAr = "المدير العام",
        NameEn = "General Admin",
        Email = "test1@gmail.com",
        UserName = "test1@gmail.com",
        EmailConfirmed = true,
        PhoneNumber = "+963912345671",
        PhoneNumberConfirmed = true
    };

    // 2. Customer 1 (الأساسي)
    public static ApplicationUser Customer => new()
    {
        Id = Guid.Parse("12345678-1234-1234-1234-000012345672"),
        NameAr = "أحمد محمد",
        NameEn = "Ahmed Mohammed",
        Email = "test2@gmail.com",
        UserName = "test2@gmail.com",
        EmailConfirmed = true,
        PhoneNumber = "+963912345672",
        PhoneNumberConfirmed = true
    };

    // 3. Owner 1
    public static ApplicationUser Owner1 => new()
    {
        Id = Guid.Parse("12345678-1234-1234-1234-000012345673"),
        NameAr = "صاحب المطعم الأول",
        NameEn = "First Restaurant Owner",
        Email = "test3@gmail.com",
        UserName = "test3@gmail.com",
        EmailConfirmed = true,
        PhoneNumber = "+963912345673",
        PhoneNumberConfirmed = true
    };

    // 4. Owner 2
    public static ApplicationUser Owner2 => new()
    {
        Id = Guid.Parse("12345678-1234-1234-1234-000012345674"),
        NameAr = "صاحب المطعم الثاني",
        NameEn = "Second Restaurant Owner",
        Email = "test4@gmail.com",
        UserName = "test4@gmail.com",
        EmailConfirmed = true,
        PhoneNumber = "+963912345674",
        PhoneNumberConfirmed = true
    };

    // 5. Customer 2
    public static ApplicationUser Customer2 => new()
    {
        Id = Guid.Parse("12345678-1234-1234-1234-000012345675"),
        NameAr = "سامر خليل",
        NameEn = "Samer Khalil",
        Email = "test5@gmail.com",
        UserName = "test5@gmail.com",
        EmailConfirmed = true,
        PhoneNumber = "+963912345675",
        PhoneNumberConfirmed = true
    };

    // 6. Customer 3
    public static ApplicationUser Customer3 => new()
    {
        Id = Guid.Parse("12345678-1234-1234-1234-000012345676"),
        NameAr = "ليلى حسن",
        NameEn = "Layla Hassan",
        Email = "test6@gmail.com",
        UserName = "test6@gmail.com",
        EmailConfirmed = true,
        PhoneNumber = "+963912345676",
        PhoneNumberConfirmed = true
    };

    // 7. Customer 4
    public static ApplicationUser Customer4 => new()
    {
        Id = Guid.Parse("12345678-1234-1234-1234-000012345677"),
        NameAr = "عمر يوسف",
        NameEn = "Omar Youssef",
        Email = "test7@gmail.com",
        UserName = "test7@gmail.com",
        EmailConfirmed = true,
        PhoneNumber = "+963912345677",
        PhoneNumberConfirmed = true
    };

    // 8. Driver 1
    public static ApplicationUser Driver1 => new()
    {
        Id = Guid.Parse("12345678-1234-1234-1234-000012345678"),
        NameAr = "السائق الأول",
        NameEn = "First Driver",
        Email = "test8@gmail.com",
        UserName = "test8@gmail.com",
        EmailConfirmed = true,
        PhoneNumber = "+963912345678",
        PhoneNumberConfirmed = true
    };

    // 9. Driver 2
    public static ApplicationUser Driver2 => new()
    {
        Id = Guid.Parse("12345678-1234-1234-1234-000012345679"),
        NameAr = "السائق الثاني",
        NameEn = "Second Driver",
        Email = "test9@gmail.com",
        UserName = "test9@gmail.com",
        EmailConfirmed = true,
        PhoneNumber = "+963912345679",
        PhoneNumberConfirmed = true
    };

    // 10. Driver 3
    public static ApplicationUser Driver3 => new()
    {
        Id = Guid.Parse("12345678-1234-1234-1234-000012345680"),
        NameAr = "السائق الثالث",
        NameEn = "Third Driver",
        Email = "test10@gmail.com",
        UserName = "test10@gmail.com",
        EmailConfirmed = true,
        PhoneNumber = "+963912345680",
        PhoneNumberConfirmed = true
    };

    // 11. Agent
    public static ApplicationUser Agent => new()
    {
        Id = Guid.Parse("12345678-1234-1234-1234-000012345681"),
        NameAr = "وكيل الدعم",
        NameEn = "Support Agent",
        Email = "test11@gmail.com",
        UserName = "test11@gmail.com",
        EmailConfirmed = true,
        PhoneNumber = "+963912345681",
        PhoneNumberConfirmed = true
    };
}
