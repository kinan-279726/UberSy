using MediatR;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Roles.Queries.GetUsersByRole;

public sealed class GetUsersByRoleQueryHandler(IOwnershipValidator ownership, IUserRoleService userRoles)
    : IRequestHandler<GetUsersByRoleQuery, Result<List<ApplicationUserGuidDto>>>
{
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly IUserRoleService _userRoles = userRoles;

    public async Task<Result<List<ApplicationUserGuidDto>>> Handle(GetUsersByRoleQuery query, CancellationToken ct)
    {
        if (!_ownership.IsInRole(Domain.Identity.Roles.Admin)) return ApplicationErrors.AccessDenied;

        return await _userRoles.GetUsersInRoleAsync(Domain.Identity.Roles.Normalize(query.Role)!, query.Page, query.PageSize, ct);
    }
}
