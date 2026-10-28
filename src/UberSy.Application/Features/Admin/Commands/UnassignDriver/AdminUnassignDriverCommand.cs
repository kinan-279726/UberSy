using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Admin.Commands.UnassignDriver;

public sealed record AdminUnassignDriverCommand(Guid ServiceRequestId) : IRequest<Result<Updated>>;