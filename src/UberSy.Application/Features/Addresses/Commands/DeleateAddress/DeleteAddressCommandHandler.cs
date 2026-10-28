using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Addresses.Commands.DeleateAddress;

public sealed class DeleteAddressCommandHandler(IApplicationDbContext context , IOwnershipValidator ownership)
    : IRequestHandler<DeleteAddressCommand, Result<Deleted>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<Deleted>> Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await _context.TbAddresses.FirstOrDefaultAsync(a => a.Id == request.AddressId, cancellationToken);
        if (address is null) return ApplicationErrors.AddressNotFound;

        var userId = await _ownership.EnsureCanEditAddress(address.Id, cancellationToken);
        if (userId.IsError) return userId.Errors;

        var result = address.Delete();

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Deleted;
    }
}