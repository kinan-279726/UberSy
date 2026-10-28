using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.ServiceRequests.Enums;
using UberSy.Domain.ServiceRequests.Events.LifecycleEvents;

namespace UberSy.Application.Features.ServiceRequests.EventHandlers;

public sealed class ServiceRequestPreparingHandler(
    IApplicationDbContext context,
    IDriverDispatchService dispatchService,
    ILogger<ServiceRequestPreparingHandler> logger)
    : INotificationHandler<ServiceRequestPreparing>
{
    private readonly IApplicationDbContext _context = context;
    private readonly IDriverDispatchService _dispatchService = dispatchService;
    private readonly ILogger<ServiceRequestPreparingHandler> _logger = logger;

    public async Task Handle(ServiceRequestPreparing notification, CancellationToken cancellationToken)
    {
        var serviceRequest = await _context.TbServiceRequests
           .AsNoTracking()
           .FirstOrDefaultAsync(s => s.Id == notification.ServiceRequestId, cancellationToken);

        if (serviceRequest is null || serviceRequest.ServiceType != ServiceType.Food)
        {
            return;
        }

        var result = await _dispatchService.DispatchNextBatchAsync(notification.ServiceRequestId, cancellationToken );

        if (result.IsError)
        {
            _logger.LogWarning("Initial Dispatch Failed For '{Id}': {Error}",
                notification.ServiceRequestId, result.TopError.Description);
        }
    }
}
