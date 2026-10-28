using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Roles.Queries.GetAllRoles;
public sealed record GetAllRolesQuery : IRequest<Result<List<string>>>;
