using MediatR;
using Microsoft.EntityFrameworkCore;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.Drivers.Commands.GoOffline;

public sealed class GoOfflineCommandHandler(IApplicationDbContext context, IUser user) : IRequestHandler<GoOfflineCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUser _user = user;

    public async Task<Result<Updated>> Handle(GoOfflineCommand request, CancellationToken cancellationToken)
    {
        var isDriver = _user.IsInRole(Domain.Identity.Roles.Driver);

        var userId = _user.Id;

        if (!isDriver || userId is null) return ApplicationErrors.AccessDenied;

        var driver = await _context.TbDrivers.FirstOrDefaultAsync(d => d.UserId == Guid.Parse(userId), cancellationToken);

        if (driver is null) return ApplicationErrors.DriverNotFound;

        if (!driver.IsApproved) return ApplicationErrors.DriverNotApproved;

        //var hasActiveOrder = await _context.TbFoodOrders.AnyAsync(
        //    o => o.DriverId == driver.Id &&
        //    (o.IdNavigation.Status != ServiceRequestStatus.Cancelled && o.IdNavigation.Status != ServiceRequestStatus.Delivered));

        var hasActiveOrder = await (
                    from o in _context.TbFoodOrders
                    join sr in _context.TbServiceRequests on o.Id equals sr.Id
                    where o.DriverId == driver.Id
                            && sr.Status != ServiceRequestStatus.Cancelled
                            && sr.Status != ServiceRequestStatus.Delivered
                    select o
                                    ).AnyAsync(cancellationToken);

        if (hasActiveOrder) return ApplicationErrors.DriverAlreadyOnDelivery;

        var offlineResult = driver.GoOffline();
        if (offlineResult.IsError) return offlineResult.Errors;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}