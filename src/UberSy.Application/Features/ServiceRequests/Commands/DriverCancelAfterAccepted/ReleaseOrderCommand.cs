using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Commands.DriverCancelAfterAccepted;

public sealed record ReleaseOrderCommand(Guid ServiceRequestId) : IRequest<Result<Updated>>;
