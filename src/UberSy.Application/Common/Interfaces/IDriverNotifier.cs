namespace UberSy.Application.Common.Interfaces;

public interface IDriverNotifier
{
    Task NotifyOfferAsync(Guid driverId, Guid offerId, Guid serviceRequestId, CancellationToken cancellation = default);

    Task NotifyOfferLostAsync(Guid driverId, Guid offerId, Guid serviceRequestId, CancellationToken cancellation = default);

    Task NotifyOrderCanceldAsync(Guid driverId, CancellationToken cancellation = default);
}
