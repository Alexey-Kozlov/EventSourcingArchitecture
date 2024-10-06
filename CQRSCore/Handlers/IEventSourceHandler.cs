using CQRSCore.Domain;

namespace CQRSCore.Handlers;

public interface IEventSourceHandler<T>
{
    Task SaveAsync(AggregateRoot aggregate);
    Task<T> GetByIdAsync(Guid aggregateId);
    Task RepublishEventsAsync();
}