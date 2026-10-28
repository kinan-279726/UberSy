using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Identity.Commands.SendPhoneCode;
public sealed record SendPhoneCodeCommand : IRequest<Result<Updated>>;
