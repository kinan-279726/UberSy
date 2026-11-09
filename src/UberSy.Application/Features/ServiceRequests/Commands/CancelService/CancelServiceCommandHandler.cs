using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Errors;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.Common.Results;
using UberSy.Domain.Identity;
using UberSy.Domain.ServiceRequests.Enums;

namespace UberSy.Application.Features.ServiceRequests.Commands.CancelService;

public sealed class CancelServiceCommandHandler(IApplicationDbContext context, IRestaurantNotifier notifier, ILogger<CancelServiceCommandHandler> logger, IUser user)
    : IRequestHandler<CancelServiceCommand, Result<Updated>>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IRestaurantNotifier _notifier = notifier;
    private readonly ILogger<CancelServiceCommandHandler> _logger = logger;
    private readonly IUser _user = user;

    public async Task<Result<Updated>> Handle(CancelServiceCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_user.Id)) return ApplicationErrors.Unauthorized;

        var service = await _context.TbServiceRequests
            .FirstOrDefaultAsync(s => s.Id == request.ServiceId, cancellationToken);

        if (service is null) return ApplicationErrors.ServiceRequestNotFound;

        var isAdmin = _user.IsInRole(Roles.Admin);

        if (!isAdmin)
        {
            if (!Guid.TryParse(_user.Id, out var userId) || userId != service.CustomerId)
                return ApplicationErrors.AccessDenied;

            if (service.Status is ServiceRequestStatus.Preparing or ServiceRequestStatus.OutForDelivery)
                return ApplicationErrors.OrderAlreadyAccepted;
        }

        var cancelResult = service.Cancel(request.Reason);
        if (cancelResult.IsError) return cancelResult.Errors;

        _logger.LogInformation("service with id {id} cancelled by user {userId}", service.Id, _user.Id);

        await _context.SaveChangesAsync(cancellationToken);

        var foodOrder = await _context.TbFoodOrders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == request.ServiceId, cancellationToken);

        if (foodOrder is not null)
            await _notifier.NotifyOrderCancledAsync(foodOrder.RestaurantId, foodOrder.Id, cancellationToken);

        return Result.Updated;
    }
}
