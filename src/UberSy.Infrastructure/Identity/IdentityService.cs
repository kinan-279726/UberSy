using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Identity;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Infrastructure.Identity;

public class IdentityService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
    IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
    IAuthorizationService authorizationService) : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
    private readonly IAuthorizationService _authorizationService = authorizationService;

    public async Task<Result<ApplicationUserDto>> AuthenticateAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return Error.NotFound("User_Not_Found", $"User with email {UtilityService.MaskEmail(email)} not found");
        }

        if (user.BannedUntil is not null && user.BannedUntil > DateTime.UtcNow)
        {
            return ApplicationErrors.UserBanned;
        }

        if (!user.EmailConfirmed)
        {
            return Error.Conflict("Email_Not_Confirmed", $"email '{UtilityService.MaskEmail(email)}' not confirmed");
        }

        if (!await _userManager.CheckPasswordAsync(user, password))
        {
            return Error.Conflict("Invalid_Login_Attempt", "Email / Password are incorrect");
        }

        var roles = await _userManager.GetRolesAsync(user!);

        var claims = await _userManager.GetClaimsAsync(user!);

        var permission = claims.Where(c => c.Type == "permission")
                                .Select(v => v.Value)
                                .ToList();

        return new ApplicationUserDto(user.Id.ToString(), user.Email!, roles, permission, claims);
    }

    public async Task<bool> AuthorizeAsync(Guid userId, string? policyName)
    {

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            return false;
        }

        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

        var result = await _authorizationService.AuthorizeAsync(principal, policyName!);

        return result.Succeeded;
    }

    public async Task<Result<ApplicationUserDto>> GetUserByIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
        {
            return ApplicationErrors.UserNotFound;
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());

        var roles = await _userManager.GetRolesAsync(user!);

        var claims = await _userManager.GetClaimsAsync(user!);

        var permission = claims.Where(c => c.Type == "permission")
                                .Select(v => v.Value)
                                .ToList();

        return new ApplicationUserDto(user!.Id.ToString(), user.Email!, roles, permission, claims);
    }

    public async Task<string?> GetUserNameAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        return user?.UserName;
    }

    public async Task<bool> IsInRoleAsync(Guid userId, string role)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());

        return user != null && await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<Result<ApplicationUserDto>> RegisterAsync(string email, string password, string nameAr, string nameEn, string phone, string role = Roles.Customer)
    {
        email = email.Trim().ToLowerInvariant();
        nameAr = nameAr.Trim();
        nameEn = nameEn.Trim();
        phone = phone.Trim();


        var existingByEmail = await _userManager.FindByEmailAsync(email);
        if (existingByEmail is not null)
            return ApplicationErrors.EmailAlreadyExists;

        var existingByPhone = _userManager.Users.ToList().FirstOrDefault(e => e.PhoneNumber == phone);
        if (existingByPhone is not null)
            return ApplicationErrors.PhoneAlreadyExists;

        var roleExists = await _roleManager.RoleExistsAsync(role);
        if (!roleExists)
            return ApplicationErrors.RoleNotFound;

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            PhoneNumber = phone,
            NameAr = nameAr,
            NameEn = nameEn,
            EmailConfirmed = false,
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = true
        };

        var createResult = await _userManager.CreateAsync(user, password);

        if (!createResult.Succeeded)
        {
            var passwordErrors = createResult.Errors
                .Where(e => e.Code.StartsWith("Password"))
                .ToList();

            if (passwordErrors.Any())
                return ApplicationErrors.PasswordTooWeak;

            var duplicateErrors = createResult.Errors
                .Where(e => e.Code == "DuplicateEmail" || e.Code == "DuplicateUserName")
                .ToList();

            if (duplicateErrors.Any())
                return ApplicationErrors.EmailAlreadyExists;

            return ApplicationErrors.UserCreationFailed;
        }

        var roleResult = await _userManager.AddToRoleAsync(user, role);

        if (!roleResult.Succeeded)
        {
            await _userManager.DeleteAsync(user);

            return ApplicationErrors.RoleAssignmentFailed;
        }


        var roles = await _userManager.GetRolesAsync(user);

        var claims = await _userManager.GetClaimsAsync(user);

        var dto = new ApplicationUserDto(
            UserId: user.Id.ToString(),
            Email: user.Email!,
            Roles: roles.ToList(),
            Permission: [],
            Claims: claims);

        return dto;
    }
}

