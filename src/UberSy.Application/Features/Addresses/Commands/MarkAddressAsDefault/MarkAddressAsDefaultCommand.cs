using MediatR;
using UberSy.Domain.Common.Results;

public sealed record MarkAddressAsDefaultCommand(Guid AddressId) : IRequest<Result<Updated>>;
