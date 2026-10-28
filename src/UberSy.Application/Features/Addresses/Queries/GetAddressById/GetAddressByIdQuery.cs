using MediatR;
using UberSy.Application.Features.Addresses.Dtos;
using UberSy.Domain.Common.Results;

public sealed record GetAddressByIdQuery(Guid AddressId) : IRequest<Result<AddressDto>>;
