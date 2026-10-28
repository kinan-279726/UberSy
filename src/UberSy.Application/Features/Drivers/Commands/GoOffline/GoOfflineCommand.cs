using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Drivers.Commands.GoOffline;

public sealed record GoOfflineCommand : IRequest<Result<Updated>>;
