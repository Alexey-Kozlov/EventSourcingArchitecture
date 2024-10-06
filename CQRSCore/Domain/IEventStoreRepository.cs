using CQRSCore.Events;

namespace CQRSCore.Domain;

public interface IEventStoreRepository
{
    Task SaveAsync(EventModel _event);
    Task<List<EventModel>> FindByAggregateId(Guid aggregateId);
    Task<List<EventModel>> FindAllAsync();
}