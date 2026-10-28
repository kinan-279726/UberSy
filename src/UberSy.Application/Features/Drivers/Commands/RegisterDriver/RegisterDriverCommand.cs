using MediatR;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.DeliveryRule.Enums;

namespace UberSy.Application.Features.Drivers.Commands.RegisterDriver;
public sealed record RegisterDriverCommand(
    VehicleType VehicleType,
    CarType? CarType,
    string NationalId,
    string? PhotoUrl,
    string? LicenseNumber,
    string? PlateNumber,
    string? Model,
    string? Color)
    : IRequest<Result<DriverDto>>;
