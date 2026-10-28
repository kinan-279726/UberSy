using MediatR;
using UberSy.Application.Features.Addresses.Dtos;
using UberSy.Domain.Common.Results;

public sealed record GetAddressesByUserQuery() : IRequest<Result<List<AddressDto>>>;
