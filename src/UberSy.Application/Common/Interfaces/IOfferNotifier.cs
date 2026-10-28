namespace UberSy.Application.Common.Interfaces;

public interface IOfferNotifier
{
    Task NotifyServiceStatusChange(CancellationToken cancellationToken = default);
}
