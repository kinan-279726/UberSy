using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Drivers.Enums;
using UberSy.Domain.Identity;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.Admin.Commands.UnassignDriver;

public sealed class AdminUnassignDriverCommandHandler(
    IApplicationDbContext context,
    IUser user,
    IDriverNotifier notifier,
    ILogger<AdminUnassignDriverCommandHandler> logger)
    : IRequestHandler<AdminUnassignDriverCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUser _user = user;
    private readonly IDriverNotifier _notifier = notifier;
    private readonly ILogger<AdminUnassignDriverCommandHandler> _logger = logger;

    public async Task<Result<Updated>> Handle(AdminUnassignDriverCommand request, CancellationToken cancellationToken)
    {
        if (!_user.IsInRole(Roles.Admin)) return ApplicationErrors.AccessDenied;

        var order = await _context.TbFoodOrders
            .Include(o => o.IdNavigation)
            .FirstOrDefaultAsync(o => o.Id == request.ServiceRequestId, cancellationToken);

        if (order is null) return ApplicationErrors.ServiceRequestNotFound;

        if (order.IdNavigation.Status != ServiceRequestStatus.Preparing)
        {
            return ApplicationErrors.CannotReleaseOrderAfterPickup;
        }

        var previousDriverId = order.DriverId;

        var removeResult = order.RemoveDriver();
        if (removeResult.IsError) return removeResult.Errors;

        var driver = await _context.TbDrivers
            .FirstOrDefaultAsync(d => d.Id == previousDriverId, cancellationToken);

        if (driver is { Status: DriverStatus.OnDelivery })
        {
            driver.FinishDelivery();
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Admin '{AdminId}' Unassigned Driver '{DriverId}' From Order '{Id}'.",
            _user.Id, previousDriverId, order.Id);

        if (driver is not null)
            await _notifier.NotifyOrderCanceldAsync(driver.Id, cancellationToken);

        return Result.Updated;
    }
}