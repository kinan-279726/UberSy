using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Commands.MarkDelivered;

public sealed record MarkDeliveredCommand(Guid ServiceRequestId) : IRequest<Result<Updated>>;
