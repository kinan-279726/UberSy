using System.ComponentModel.DataAnnotations.Schema;
using UberSy.Domain.Common.Results;

namespace UberSy.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; }

    public bool IsDeleted { get; set; }

    private readonly List<DomainEvent> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Entity()
    { }

    protected Entity(Guid id)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
    }

    public void AddDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    public void RemoveDomainEvent(DomainEvent domainEvent) => _domainEvents.Remove(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();

    public Result<Deleted> Delete()
    {
        IsDeleted = true;
        return Result.Deleted;
    }

    public Result<Updated> Restore()
    {
        IsDeleted = false;
        return Result.Updated;
    }
}
