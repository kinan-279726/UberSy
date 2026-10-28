using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Roles.Queries.GetUserRoles;

public sealed record GetUserRolesQuery(Guid UserId) : IRequest<Result<ApplicationUserGuidDto>>;
