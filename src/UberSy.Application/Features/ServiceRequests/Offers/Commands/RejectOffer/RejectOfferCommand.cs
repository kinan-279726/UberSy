using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;

namespace UberSy.Application.Features.ServiceRequests.Offers.Commands.RejectOffer;

public sealed record RejectOfferCommand(Guid OfferId) : IRequest<Result<Updated>>;

public sealed class RejectOfferCommandHandler(IApplicationDbContext context, IOwnershipValidator ownership)
    : IRequestHandler<RejectOfferCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<Updated>> Handle(RejectOfferCommand request, CancellationToken cancellationToken)
    {
        var driverResult = await _ownership.IsDriver();
        if (driverResult.IsError) return driverResult.Errors;

        var offer = await _context.TbServiceRequestOffers.FirstOrDefaultAsync(
            o => o.Id == request.OfferId,
            cancellationToken);

        if (offer is null) return ApplicationErrors.OfferNotFound;

        if (offer.DriverId != driverResult.Value) return ApplicationErrors.AccessDenied;

        var offerResult = offer.Reject();
        if (offerResult.IsError) return offerResult.Errors;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }

}
