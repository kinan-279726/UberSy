using UberSy.Application.Features.Identity;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Common.Interfaces;

public interface IUserRoleService
{
    Task<Result<ApplicationUserGuidDto>> GetUserRolesAsync(Guid userId, CancellationToken ct = default);
    Task<Result<Updated>> AddToRoleAsync(Guid userId, string role);
    Task<Result<Updated>> RemoveFromRoleAsync(Guid userId, string role);
    Task<List<ApplicationUserGuidDto>> GetUsersInRoleAsync(string role, int page, int pageSize, CancellationToken ct = default);
    Task<int> CountUsersInRoleAsync(string role, CancellationToken ct = default);
}
