using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Commands.AcceptFoodOrder;

public sealed record AcceptOrderCommand(Guid ServiceRequestId) : IRequest<Result<Updated>>;
