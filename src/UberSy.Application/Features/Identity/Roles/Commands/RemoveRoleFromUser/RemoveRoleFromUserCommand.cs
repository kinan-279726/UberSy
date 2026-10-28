using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Roles.Commands.RemoveRoleFromUser;

public sealed record RemoveRoleFromUserCommand(Guid UserId, string Role) : IRequest<Result<ApplicationUserGuidDto>>;
