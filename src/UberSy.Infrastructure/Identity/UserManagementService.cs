using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Identity.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Infrastructure.Data;

namespace UberSy.Infrastructure.Identity;

public sealed class UserManagementService(
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext context) : IUserManagementService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly ApplicationDbContext _context = context;

    public async Task<PagedList<AdminUserListItemDto>> GetUsersAsync(
        string? search, string? role, bool? isBanned, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var query = _context.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(u => u.Email!.Contains(term)
                                     || u.NameAr.Contains(term)
                                     || u.NameEn.Contains(term)
                                     || (u.PhoneNumber != null && u.PhoneNumber.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            var normalized = role.ToUpperInvariant();
            query = query.Where(u => _context.UserRoles.Any(ur =>
                ur.UserId == u.Id
                && _context.Roles.Any(r => r.Id == ur.RoleId && r.NormalizedName == normalized)));
        }

        if (isBanned == true)
            query = query.Where(u => u.BannedUntil != null && u.BannedUntil > now);
        else if (isBanned == false)
            query = query.Where(u => u.BannedUntil == null || u.BannedUntil <= now);

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderBy(u => u.Email)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var ids = users.Select(u => u.Id).ToList();

        var roleRows = await (
            from ur in _context.UserRoles
            join r in _context.Roles on ur.RoleId equals r.Id
            where ids.Contains(ur.UserId)
            select new { ur.UserId, r.Name })
            .ToListAsync(cancellationToken);

        var items = users.Select(u => new AdminUserListItemDto
        {
            Id = u.Id,
            Email = u.Email,
            NameAr = u.NameAr,
            NameEn = u.NameEn,
            Phone = u.PhoneNumber,
            EmailConfirmed = u.EmailConfirmed,
            PhoneConfirmed = u.PhoneNumberConfirmed,
            BannedUntil = u.BannedUntil,
            Roles = roleRows.Where(r => r.UserId == u.Id).Select(r => r.Name!).ToList(),
        }).ToList();

        return PagedList<AdminUserListItemDto>.Create(items, pageNumber, pageSize, totalCount);
    }

    public async Task<Result<Updated>> BanAsync(Guid userId, DateTime until, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return ApplicationErrors.UserNotFound;

        user.BannedUntil = until;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return ApplicationErrors.UserCreationFailed;

        await _context.RefreshTokens
            .Where(rt => rt.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Updated;
    }

    public async Task<Result<Updated>> UnbanAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is null) return ApplicationErrors.UserNotFound;

        user.BannedUntil = null;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return ApplicationErrors.UserCreationFailed;

        return Result.Updated;
    }

    public async Task<UserContactDto?> FindContactByPhoneAsync(string phone, CancellationToken cancellationToken = default)
    {
        var candidates = BuildPhoneCandidates(phone);

        var found = await _context.Users
            .AsNoTracking()
            .Where(u => u.PhoneNumber != null && candidates.Contains(u.PhoneNumber))
            .Select(u => new UserContactDto
            {
                UserId = u.Id,
                NameAr = u.NameAr,
                NameEn = u.NameEn,
                Phone = u.PhoneNumber,
            })
            .FirstOrDefaultAsync(cancellationToken);

        return found;
    }

    public async Task<UserContactDto?> GetContactAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserContactDto
            {
                UserId = u.Id,
                NameAr = u.NameAr,
                NameEn = u.NameEn,
                Phone = u.PhoneNumber,
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static List<string> BuildPhoneCandidates(string phone)
    {
        var value = phone.Trim().Replace(" ", "").Replace("-", "");
        var candidates = new List<string> { value };

        string local;
        if (value.StartsWith("+963")) local = value[4..];
        else if (value.StartsWith("00963")) local = value[5..];
        else if (value.StartsWith("963")) local = value[3..];
        else if (value.StartsWith('0')) local = value[1..];
        else local = value;

        candidates.Add(local);
        candidates.Add("0" + local);
        candidates.Add("963" + local);
        candidates.Add("+963" + local);

        return candidates.Distinct().ToList();
    }
}