using MediatR;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Roles.Queries.GetUserRoles;

public sealed class GetUserRolesQueryHandler(IOwnershipValidator ownership, IUserRoleService userRoles)
    : IRequestHandler<GetUserRolesQuery, Result<ApplicationUserGuidDto>>
{
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly IUserRoleService _userRoles = userRoles;

    public async Task<Result<ApplicationUserGuidDto>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        if (!_ownership.IsInRole(Domain.Identity.Roles.Admin)) return ApplicationErrors.AccessDenied;

        return await _userRoles.GetUserRolesAsync(request.UserId, cancellationToken);
    }
}