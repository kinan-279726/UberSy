using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Application.Features.Drivers.Dtos;
using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.Drivers.Queries.GetDriverLocation;

public sealed class GetDriverLocationQueryHandler(IApplicationDbContext context, IOwnershipValidator ownership)
    : IRequestHandler<GetDriverLocationQuery, Result<DriverLocationDto>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;

    public async Task<Result<DriverLocationDto>> Handle(GetDriverLocationQuery request, CancellationToken cancellationToken)
    {
        var userResult = await _ownership.IsUser();
        if (userResult.IsError) return userResult.Errors;

        var order = await _context.TbFoodOrders.AsNoTracking()
            .Where(o => o.Id == request.ServiceRequestId)
            .Select(o => new { o.DriverId, o.IdNavigation.CustomerId, o.IdNavigation.Status })
            .FirstOrDefaultAsync(cancellationToken);

        if (order is null) return ApplicationErrors.ServiceRequestNotFound;
        if (order.CustomerId != userResult.Value) return ApplicationErrors.AccessDenied;
        if (order.DriverId is null
            || order.Status is not (ServiceRequestStatus.Preparing or ServiceRequestStatus.OutForDelivery))
            return ApplicationErrors.ServiceRequestNotFound;

        var loc = await _context.TbDriverLocations.AsNoTracking()
            .FirstOrDefaultAsync(l => l.DriverId == order.DriverId, cancellationToken);
        if (loc is null) return ApplicationErrors.DriverNotFound;

        return new DriverLocationDto(loc.Boundary.Y, loc.Boundary.X, loc.UpdatedAt);
    }
}