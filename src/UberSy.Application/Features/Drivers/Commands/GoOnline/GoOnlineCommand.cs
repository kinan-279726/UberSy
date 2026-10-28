using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Drivers.Commands.GoOnline;

public sealed record GoOnlineCommand : IRequest<Result<Updated>>;

