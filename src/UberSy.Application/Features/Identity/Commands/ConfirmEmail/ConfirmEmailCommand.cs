using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(string Email, string Code) : IRequest<Result<Updated>>;
