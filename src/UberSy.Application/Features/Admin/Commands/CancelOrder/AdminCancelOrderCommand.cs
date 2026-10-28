using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Admin.Commands.CancelOrder;

public sealed record AdminCancelOrderCommand(Guid ServiceRequestId, string Reason) : IRequest<Result<Updated>>;