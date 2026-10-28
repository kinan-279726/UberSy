using Microsoft.AspNetCore.Identity;

namespace UberSy.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string NameAr { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public DateTime? BannedUntil { get; set; }
}
