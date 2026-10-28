using MediatR;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Drivers.Queries.GetDriverLocation;

public sealed record GetDriverLocationQuery(Guid ServiceRequestId) : IRequest<Result<DriverLocationDto>>;
