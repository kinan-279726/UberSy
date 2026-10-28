using MediatR;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Application.Features.Admin.Commands.BanUser;

public sealed class BanUserCommandHandler(
    IUserManagementService users,
    IUserRoleService userRoles,
    IUser user,
    ILogger<BanUserCommandHandler> logger)
    : IRequestHandler<BanUserCommand, Result<Updated>>
{
    private readonly IUserManagementService _users = users;
    private readonly IUserRoleService _userRoles = userRoles;
    private readonly IUser _user = user;
    private readonly ILogger<BanUserCommandHandler> _logger = logger;

    public async Task<Result<Updated>> Handle(BanUserCommand request, CancellationToken cancellationToken)
    {
        if (!_user.IsInRole(Roles.Admin)) return ApplicationErrors.AccessDenied;

        if (request.UserId.ToString() == _user.Id) return ApplicationErrors.CannotBanSelf;

        var rolesResult = await _userRoles.GetUserRolesAsync(request.UserId, cancellationToken);
        if (rolesResult.IsError) return rolesResult.Errors;

        if (rolesResult.Value.Roles!.Contains(Roles.Admin)) return ApplicationErrors.CannotBanAdmin;
       // if (rolesResult.Value.Roles?.Contains(Domain.Identity.Roles.Admin) == true) return ApplicationErrors.CannotBanAdmin;

        var until = request.Until ?? DateTime.UtcNow.AddYears(100);

        var result = await _users.BanAsync(request.UserId, until, cancellationToken);
        if (result.IsError) return result.Errors;

        _logger.LogWarning("User '{UserId}' Banned Until {Until} By Admin '{AdminId}'.",
            request.UserId, until, _user.Id);

        return Result.Updated;
    }
}