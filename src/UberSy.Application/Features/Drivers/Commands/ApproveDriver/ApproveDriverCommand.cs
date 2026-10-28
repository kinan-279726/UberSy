using MediatR;
using UberSy.Domain.Common.Results;
using UberSy.Domain.DeliveryRule.Enums;

namespace UberSy.Application.Features.Drivers.Commands.ApproveDriver;

public sealed record ApproveDriverCommand(Guid DriverId, ServiceTier ServiceTier) : IRequest<Result<Updated>>;
