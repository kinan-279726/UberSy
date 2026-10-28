using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Commands.ConfirmPhone;
public sealed record ConfirmPhoneCommand(string Code) : IRequest<Result<Updated>>;
