using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Commands.RejectFoodOrder;

public sealed record RejectOrderCommand(Guid ServiceRequestId, string Reason) : IRequest<Result<Updated>>;
