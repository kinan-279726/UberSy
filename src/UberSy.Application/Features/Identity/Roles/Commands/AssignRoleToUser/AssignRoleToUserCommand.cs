using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Roles.Commands.AssignRoleToUser;

public sealed record AssignRoleToUserCommand(Guid UserId, string Role) : IRequest<Result<ApplicationUserGuidDto>>;
