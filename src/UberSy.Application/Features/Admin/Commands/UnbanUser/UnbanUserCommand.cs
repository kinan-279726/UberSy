using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Admin.Commands.UnbanUser;

public sealed record UnbanUserCommand(Guid UserId) : IRequest<Result<Updated>>;
