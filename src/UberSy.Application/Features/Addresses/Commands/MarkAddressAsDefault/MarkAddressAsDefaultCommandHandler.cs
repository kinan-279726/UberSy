using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

public sealed class MarkAddressAsDefaultCommandHandler(IApplicationDbContext context, IOwnershipValidator ownership)
    : IRequestHandler<MarkAddressAsDefaultCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;

    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<Updated>> Handle(MarkAddressAsDefaultCommand request, CancellationToken cancellationToken)
    {
        var address = await _context.TbAddresses
            .FirstOrDefaultAsync(a => a.Id == request.AddressId, cancellationToken);

        if (address is null)
        {
            return ApplicationErrors.AddressNotFound;
        }

        var userId = await _ownership.EnsureCanEditAddress(request.AddressId);
        if (userId.IsError)
        {
            return userId.Errors;
        }

        if (address.IsDefault)
        {
            return Result.Updated;
        }

        var otherDefaultAddresses = await _context.TbAddresses
            .Where(a => a.UserId == userId.Value && a.IsDefault && a.Id != request.AddressId)
            .ToListAsync(cancellationToken);

        foreach (var other in otherDefaultAddresses)
        {
            other.UnmarkAsDefault();
        }

        var result = address.MarkAsDefault();
        if (result.IsError) return result.Errors;

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}