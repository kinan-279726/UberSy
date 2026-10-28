using MediatR;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Roles.Commands.AssignRoleToUser;

public sealed class AssignRoleToUserCommandHandler(
    IOwnershipValidator ownership,
    IUserRoleService userRoles,
    ILogger<AssignRoleToUserCommandHandler> logger)
    : IRequestHandler<AssignRoleToUserCommand, Result<ApplicationUserGuidDto>>
{
    private readonly IOwnershipValidator _ownership= ownership;
    private readonly IUserRoleService _userRoles = userRoles;
    private readonly ILogger<AssignRoleToUserCommandHandler> _logger = logger;

    public async Task<Result<ApplicationUserGuidDto>> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        if (!_ownership.IsInRole(Domain.Identity.Roles.Admin)) return ApplicationErrors.AccessDenied;

        var role = Domain.Identity.Roles.Normalize(request.Role)!;

        var add = await _userRoles.AddToRoleAsync(request.UserId, role);
        if (add.IsError) return add.Errors;

        _logger.LogInformation("Role '{Role}' Assigned To User '{UserId}' By 'AdminId'.", role, request.UserId);

        return await _userRoles.GetUserRolesAsync(request.UserId, cancellationToken);
    }
}
