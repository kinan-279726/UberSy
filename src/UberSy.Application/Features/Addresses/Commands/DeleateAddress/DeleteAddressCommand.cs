using MediatR;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Addresses.Commands.DeleateAddress;
public sealed record DeleteAddressCommand(Guid AddressId) : IRequest<Result<Deleted>>;
