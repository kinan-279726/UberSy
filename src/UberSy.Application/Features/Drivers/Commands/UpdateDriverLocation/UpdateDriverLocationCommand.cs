using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Drivers.Commands.UpdateDriverLocation;

public sealed record UpdateDriverLocationCommand(Guid DriverId, double Latitude, double Longitude) : IRequest<Result<Updated>>;
