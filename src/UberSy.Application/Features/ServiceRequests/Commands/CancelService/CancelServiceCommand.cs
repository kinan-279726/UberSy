using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Commands.CancelService;

public sealed record CancelServiceCommand(Guid ServiceId) : IRequest<Result<Updated>>;
