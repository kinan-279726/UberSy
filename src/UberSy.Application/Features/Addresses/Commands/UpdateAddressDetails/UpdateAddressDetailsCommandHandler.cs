using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.Addresses.Commands.UpdateAddressDetails;

public sealed class UpdateAddressDetailsCommandHandler(IApplicationDbContext context , IOwnershipValidator ownership)
    : IRequestHandler<UpdateAddressDetailsCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<Updated>> Handle(UpdateAddressDetailsCommand request, CancellationToken cancellation)
    {
        var address = await _context.TbAddresses.FirstOrDefaultAsync(a => a.Id == request.AddressId, cancellation);
        if (address is null) return ApplicationErrors.AddressNotFound;

        var userId = await _ownership.EnsureCanEditAddress(address.Id, cancellation);
        if (userId.IsError) return userId.Errors;

        var boundary = new Point(request.Longitude, request.Latitude) { SRID = 4326 };

        var result = address.UpdateDetails(request.Street, request.City, boundary, request.Label, request.Floor, request.Phone);
        if (result.IsError) return result.Errors;

        await _context.SaveChangesAsync(cancellation);
        return Result.Updated;
    }
}