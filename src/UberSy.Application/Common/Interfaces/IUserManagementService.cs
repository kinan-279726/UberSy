using UberSy.Application.Common.Models;
using UberSy.Application.Features.Identity.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Common.Interfaces;

public interface IUserManagementService
{
    Task<PagedList<AdminUserListItemDto>> GetUsersAsync(
        string? search, string? role, bool? isBanned, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default);
    Task<Result<Updated>> BanAsync(Guid userId, DateTime until, CancellationToken cancellationToken = default);
    Task<Result<Updated>> UnbanAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<UserContactDto?> FindContactByPhoneAsync(string phone, CancellationToken cancellationToken = default);
    Task<UserContactDto?> GetContactAsync(Guid userId, CancellationToken cancellationToken = default);

}