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
        var service = await _context.TbServiceRequests.FirstOrDefaultAsync(s => s.Id == request.ServiceId, cancellationToken);

        if (service is null) return ApplicationErrors.ServiceRequestNotFound;

        if (string.IsNullOrWhiteSpace(_user.Id)) return ApplicationErrors.AccessDenied;

        if (Guid.Parse(_user.Id) != service.CustomerId)
        {
            if (!_user.IsInRole(Roles.Admin))
            {
                return ApplicationErrors.AccessDenied;
            }
        }

        if (service.Status != ServiceRequestStatus.Pending) return ApplicationErrors.OrderAlreadyAccepted;

        var canclleResult = service.Cancel("");

        if (canclleResult.IsError) return canclleResult.Errors;

        _logger.LogInformation("service with id {id} cancelled by user {userId}", service.Id, _user.Id);


        await _context.SaveChangesAsync(cancellationToken);

        var rest = await _context.TbFoodOrders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == request.ServiceId, cancellationToken);

        if (rest is not null)
            await _notifier.NotifyOrderCancledAsync(rest.RestaurantId, rest.Id);

        return Result.Updated;
    }
}