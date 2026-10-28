using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.ServiceRequests.Commands.DriverCancelAfterAccepted;

public sealed record ReleaseOrderCommand(Guid ServiceRequestId) : IRequest<Result<Updated>>;

public sealed class ReleaseOrderCommandHandler(
    IApplicationDbContext context,
    IOwnershipValidator ownership,
    ILogger<ReleaseOrderCommandHandler> logger)
    : IRequestHandler<ReleaseOrderCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IOwnershipValidator _ownership = ownership;
    private readonly ILogger<ReleaseOrderCommandHandler> _logger = logger;

    public async Task<Result<Updated>> Handle(ReleaseOrderCommand request, CancellationToken cancellationToken)
    {
        var driverResult = await _ownership.IsDriver();
        if (driverResult.IsError) return driverResult.Errors;
        var driverId = driverResult.Value;

        var order = await _context.TbFoodOrders
            .Include(o => o.IdNavigation)
            .FirstOrDefaultAsync(o => o.Id == request.ServiceRequestId, cancellationToken);

        if (order is null) return ApplicationErrors.ServiceRequestNotFound;
        if (order.DriverId != driverId) return ApplicationErrors.AccessDenied;

        if (order.IdNavigation.Status != ServiceRequestStatus.Preparing)
        {
            return ApplicationErrors.CannotReleaseOrderAfterPickup;
        }

        var removeResult = order.RemoveDriver();
        if (removeResult.IsError) return removeResult.Errors;

        var driver = await _context.TbDrivers.FirstAsync(d => d.Id == driverId, cancellationToken);

        var finishResult = driver.FinishDelivery();
        if (finishResult.IsError)
        {
            _logger.LogWarning("Could Not Reset Driver '{DriverId}' Status: {Error}",
                driverId, finishResult.TopError.Description);
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Driver '{DriverId}' Released Order '{Id}'.", driverId, request.ServiceRequestId);

        return Result.Updated;
    }
}