using MediatR;
using UberSy.Application.Features.Addresses.Dtos;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Addresses.Commands.CreateAddress;

public sealed record CreateAddressCommand(
    string Street,
    string City,
    double Latitude,
    double Longitude,
    string? Label,
    string? Floor,
    string? Phone)
    : IRequest<Result<AddressDto>>;
