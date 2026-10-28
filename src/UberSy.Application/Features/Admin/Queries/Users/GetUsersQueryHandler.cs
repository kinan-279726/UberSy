using MediatR;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Common.Models;
using UberSy.Application.Features.Identity.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Application.Features.Admin.Queries.Users;

public sealed class GetUsersQueryHandler(IUserManagementService users, IUser user)
    : IRequestHandler<GetUsersQuery, Result<PagedList<AdminUserListItemDto>>>
{
    private readonly IUserManagementService _users = users;
    private readonly IUser _user = user;

    public async Task<Result<PagedList<AdminUserListItemDto>>> Handle(
        GetUsersQuery request, CancellationToken cancellationToken)
    {
        if (!_user.IsInRole(Roles.Admin)) return ApplicationErrors.AccessDenied;

        return await _users.GetUsersAsync(
            request.Search, request.Role, request.IsBanned, request.PageNumber, request.PageSize, cancellationToken);
    }
}