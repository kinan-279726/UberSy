using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Helpers;
using System.Security.Claims;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Identity;
using UberSy.Domain.Common.Results;
using UberSy.Infrastructure.Data;

namespace UberSy.Infrastructure.Identity
{
    public sealed class UserRoleService(UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ApplicationDbContext context) : IUserRoleService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
        private readonly ApplicationDbContext _context = context;

        public async Task<Result<Updated>> AddToRoleAsync(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return ApplicationErrors.UserNotFound;

            if (!await _roleManager.RoleExistsAsync(role)) return ApplicationErrors.RoleNotFound;
            if (await _userManager.IsInRoleAsync(user, role)) return ApplicationErrors.RoleAlreadyAssigned;

            var result = await _userManager.AddToRoleAsync(user, role);
            if (!result.Succeeded) return ApplicationErrors.RoleAssignmentFailed;

            return Result.Updated;
        }

        public async Task<int> CountUsersInRoleAsync(string role, CancellationToken ct = default)
        {
            var normalized = role.ToUpperInvariant();

            return await(from ur in _context.UserRoles
                         join r in _context.Roles on ur.RoleId equals r.Id
                         where r.NormalizedName == normalized
                         select ur.UserId)
                .CountAsync(ct);
        }

        public async Task<Result<ApplicationUserGuidDto>> GetUserRolesAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return ApplicationErrors.UserNotFound;

            var roles = await _userManager.GetRolesAsync(user);

            return new ApplicationUserGuidDto(user.Id, user.Email!, roles.ToList() , [] , []);
        }

        public async Task<List<ApplicationUserGuidDto>> GetUsersInRoleAsync(string role, int page , int pageSize, CancellationToken ct = default)
        {
            var normalized = role.ToUpperInvariant();

            return await(from u in _context.Users.AsNoTracking()
                         join ur in _context.UserRoles on u.Id equals ur.UserId
                         join r in _context.Roles on ur.RoleId equals r.Id
                         where r.NormalizedName == normalized
                         orderby u.Email
                         select new ApplicationUserGuidDto(u.Id, u.Email! ,new List<string> { } , new List<string> { }, new List<Claim> { }))
           .Skip((page - 1) * pageSize)
           .Take(pageSize)
           .ToListAsync(ct);
        }

        public async Task<Result<Updated>> RemoveFromRoleAsync(Guid userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null) return ApplicationErrors.UserNotFound;

            if (!await _userManager.IsInRoleAsync(user, role)) return ApplicationErrors.RoleNotAssigned;

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (!result.Succeeded) return ApplicationErrors.RoleAssignmentFailed;

            return Result.Updated;
        }
    }
}
