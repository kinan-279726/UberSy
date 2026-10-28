using MediatR;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;

namespace UberSy.Application.Features.Admin.Commands.UnbanUser;

public sealed class UnbanUserCommandHandler(IUserManagementService users, IUser user)
    : IRequestHandler<UnbanUserCommand, Result<Updated>>
{
    private readonly IUserManagementService _users = users;
    private readonly IUser _user = user;

    public async Task<Result<Updated>> Handle(UnbanUserCommand request, CancellationToken cancellationToken)
    {
        if (!_user.IsInRole(Roles.Admin)) return ApplicationErrors.AccessDenied;

        return await _users.UnbanAsync(request.UserId, cancellationToken);
    }
}