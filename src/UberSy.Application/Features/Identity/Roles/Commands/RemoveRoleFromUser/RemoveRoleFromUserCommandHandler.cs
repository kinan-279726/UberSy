using MediatR;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Roles.Commands.RemoveRoleFromUser;

public sealed class RemoveRoleFromUserCommandHandler(
    IOwnershipValidator ownership,
    IUser user ,
    IUserRoleService userRoles,
    ILogger<RemoveRoleFromUserCommandHandler> logger)
    : IRequestHandler<RemoveRoleFromUserCommand, Result<ApplicationUserGuidDto>>
{
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly IUser _user = user;
    private readonly IUserRoleService _userRoles = userRoles;
    private readonly ILogger<RemoveRoleFromUserCommandHandler> _logger = logger;

    public async Task<Result<ApplicationUserGuidDto>> Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
    {
        if (!_ownership.IsInRole(Domain.Identity.Roles.Admin)) return ApplicationErrors.AccessDenied;

        var role = Domain.Identity.Roles.Normalize(request.Role)!;

        if (role == Domain.Identity.Roles.Admin)
        {
            if (Guid.TryParse(_user.Id, out var currentUserId) && currentUserId == request.UserId)
            {
                return ApplicationErrors.CannotRemoveOwnAdminRole;
            }

            if (await _userRoles.CountUsersInRoleAsync(Domain.Identity.Roles.Admin, cancellationToken) <= 1)
            {
                return ApplicationErrors.LastAdminCannotBeRemoved;
            }
        }

        var remove = await _userRoles.RemoveFromRoleAsync(request.UserId, role);
        if (remove.IsError) return remove.Errors;

        _logger.LogInformation("Role '{Role}' Removed From User '{UserId}' By '{AdminId}'.", role, request.UserId, _user.Id);

        return await _userRoles.GetUserRolesAsync(request.UserId, cancellationToken);
    }
}