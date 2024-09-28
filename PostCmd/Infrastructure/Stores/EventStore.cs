using CQRSCore.Domain;
using CQRSCore.Events;
using CQRSCore.Exceptions;
using CQRSCore.Infrastructure;
using MongoDB.Driver;
using PostCmd.Domain.Aggregates;

namespace PostCmd.Infrastructure.Stores;

public class EventStore : IEventStore
{
    private readonly IEventStoreRepository _eventStoreRepository;
    public EventStore(IEventStoreRepository eventStoreRepository)
    {
        _eventStoreRepository = eventStoreRepository;
    }
    public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
        if (eventStream == null || !eventStream.Any())
        {
            throw new AggregateNotFoundException("Указанный пост не найден");
        }
        return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
    }

    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);
        if (expectedVersion != -1 && eventStream[-1]?.Version != expectedVersion)
        {
            throw new ConcurencyException();
        }
        var version = expectedVersion;
        foreach (var _event in events)
        {
            version++;
            _event.Version = version;
            var eventType = _event.GetType().Name;
            var eventModel = new EventModel
            {
                TimeStamp = DateTime.Now,
                AggregateId = aggregateId,
                AggregateType = nameof(PostAggregates),
                Version = version,
                EventType = eventType,
                EventData = _event
            };
            await _eventStoreRepository.SaveAsync(eventModel);
        }
    }
}