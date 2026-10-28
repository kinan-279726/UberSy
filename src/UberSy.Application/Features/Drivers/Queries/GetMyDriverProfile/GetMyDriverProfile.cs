using MediatR;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Drivers.Queries.GetMyDriverProfile;

public sealed record GetMyDriverProfileQuery : IRequest<Result<DriverProfileDto>>;
