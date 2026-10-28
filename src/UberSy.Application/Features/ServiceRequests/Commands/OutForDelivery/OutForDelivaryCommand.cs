using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Commands.OutForDelivery;

public sealed record OutForDelivaryCommand(Guid ServiceRequestId) : IRequest<Result<Updated>>;
