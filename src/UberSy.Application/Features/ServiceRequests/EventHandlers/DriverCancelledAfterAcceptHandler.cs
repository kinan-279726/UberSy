using MediatR;
using Microsoft.Extensions.Logging;
using UberSy.Application.Common.Interfaces;
using UberSy.Domain.ServiceRequests.Events.DriverEvents;

namespace UberSy.Application.Features.ServiceRequests.EventHandlers;

public sealed class DriverCancelledAfterAcceptHandler(
    IDriverDispatchService dispatchService,
    ILogger<DriverCancelledAfterAcceptHandler> logger)
    : INotificationHandler<DriverCancelledAfterAccept>
{
    private readonly IDriverDispatchService _dispatchService = dispatchService;
    private readonly ILogger<DriverCancelledAfterAcceptHandler> _logger = logger;

    public async Task Handle(DriverCancelledAfterAccept notification, CancellationToken cancellationToken)
    {
        var result = await _dispatchService.DispatchNextBatchAsync(notification.ServiceRequestId, cancellationToken);

        if (result.IsError)
        {
            _logger.LogWarning("Re-Dispatch Failed For '{Id}': {Error}",
                notification.ServiceRequestId, result.TopError.Description);
        }
    }
}