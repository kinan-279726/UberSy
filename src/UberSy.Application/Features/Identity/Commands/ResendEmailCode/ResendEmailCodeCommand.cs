using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Commands.ResendEmailCode;
public sealed record ResendEmailCodeCommand(string Email) : IRequest<Result<Updated>>;
