using MediatR;

namespace UberSy.Domain.Common;

public abstract class DomainEvent : INotification
{
    public DateTime OccurredAtUtc { get; } = DateTime.UtcNow;
}
