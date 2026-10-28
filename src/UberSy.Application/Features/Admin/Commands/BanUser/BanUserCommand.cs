using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Admin.Commands.BanUser;

public sealed record BanUserCommand(Guid UserId, DateTime? Until) : IRequest<Result<Updated>>;
