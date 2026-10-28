using MediatR;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Roles.Queries.GetAllRoles;

public sealed class GetAllRolesQueryHandler(IOwnershipValidator ownership) : IRequestHandler<GetAllRolesQuery, Result<List<string>>>
{
    private readonly IOwnershipValidator _ownership = ownership ;

    public Task<Result<List<string>>> Handle(GetAllRolesQuery query, CancellationToken ct)
    {
        Result<List<string>> result = _ownership.IsInRole(Domain.Identity.Roles.Admin)
            ? Domain.Identity.Roles.All.ToList()
            : ApplicationErrors.AccessDenied;

        return Task.FromResult(result);
    }
}