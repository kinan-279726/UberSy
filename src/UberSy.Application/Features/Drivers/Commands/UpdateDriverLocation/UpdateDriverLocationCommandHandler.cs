using MediatR;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Drivers;
using UberSy.Domain.Drivers.Enums;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.Drivers.Commands.UpdateDriverLocation;

public sealed class UpdateDriverLocationCommandHandler(IApplicationDbContext context , IUserNotifier userNotifier)
    : IRequestHandler<UpdateDriverLocationCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUserNotifier _userNotifier = userNotifier;

    public async Task<Result<Updated>> Handle(UpdateDriverLocationCommand request, CancellationToken cancellation)
    {
        var driver = await _context.TbDrivers
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == request.DriverId, cancellation);

        if (driver is null)
        {
            return ApplicationErrors.DriverNotFound;
        }

        if (driver.Status != DriverStatus.Online)
        {
            return ApplicationErrors.DriverOffline;
        }

        var point = new Point(request.Longitude, request.Latitude) { SRID = 4326 };

        var location = await _context.TbDriverLocations
            .FirstOrDefaultAsync(l => l.DriverId == request.DriverId, cancellation);

        if (location is null)
        {
            var createResult = TbDriverLocation.Create(Guid.NewGuid(), request.DriverId, point);

            if (createResult.IsError)
            {
                return createResult.Errors;
            }

            _context.TbDriverLocations.Add(createResult.Value);
        }
        else
        {
            var updateResult = location.UpdateLocation(point);

            if (updateResult.IsError)
            {
                return updateResult.Errors;
            }
        }

        await _context.SaveChangesAsync(cancellation);

        var active = await _context.TbFoodOrders.AsNoTracking()
            .Where(o => o.DriverId == request.DriverId
                && (o.IdNavigation.Status == ServiceRequestStatus.Preparing
                    || o.IdNavigation.Status == ServiceRequestStatus.OutForDelivery))
            .Select(o => new { o.Id, o.IdNavigation.CustomerId })
            .FirstOrDefaultAsync(cancellation);

        if (active is not null)
        {
            await _userNotifier.NotifyDriverLocationAsync(
                active.Id, active.CustomerId, request.Latitude, request.Longitude, cancellation);
        }

        return Result.Updated;
    }
}