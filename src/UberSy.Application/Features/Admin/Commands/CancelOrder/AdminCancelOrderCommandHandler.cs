using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Drivers.Enums;
using UberSy.Domain.Identity;
using UberSy.Domain.ServiceRequests.Offers.Enums;

namespace UberSy.Application.Features.Admin.Commands.CancelOrder;

public sealed class AdminCancelOrderCommandHandler(
    IApplicationDbContext context,
    IUser user,
    ILogger<AdminCancelOrderCommandHandler> logger)
    : IRequestHandler<AdminCancelOrderCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IUser _user = user;
    private readonly ILogger<AdminCancelOrderCommandHandler> _logger = logger;

    public async Task<Result<Updated>> Handle(AdminCancelOrderCommand request, CancellationToken cancellationToken)
    {
        if (!_user.IsInRole(Roles.Admin)) return ApplicationErrors.AccessDenied;

        var order = await _context.TbFoodOrders
            .Include(o => o.IdNavigation)
            .FirstOrDefaultAsync(o => o.Id == request.ServiceRequestId, cancellationToken);

        if (order is null) return ApplicationErrors.ServiceRequestNotFound;

        var cancelResult = order.IdNavigation.Cancel(request.Reason);
        if (cancelResult.IsError) return cancelResult.Errors;

        if (order.DriverId is not null)
        {
            var driver = await _context.TbDrivers
                .FirstOrDefaultAsync(d => d.Id == order.DriverId, cancellationToken);

            if (driver is { Status: DriverStatus.OnDelivery })
            {
                driver.FinishDelivery();
            }
        }

        var pendingOffers = await _context.TbServiceRequestOffers
            .Where(o => o.ServiceRequestId == order.Id && o.Status == OfferStatus.Sent)
            .ToListAsync(cancellationToken);

        foreach (var offer in pendingOffers)
        {
            offer.Expire();
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Order '{Id}' Cancelled By Admin '{AdminId}'. Reason: {Reason}",
            order.Id, _user.Id, request.Reason);

        return Result.Updated;
    }
}
