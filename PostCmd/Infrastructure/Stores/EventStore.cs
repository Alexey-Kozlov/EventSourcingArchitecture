using CQRSCore.Domain;
using CQRSCore.Events;
using CQRSCore.Exceptions;
using CQRSCore.Infrastructure;
using CQRSCore.Producers;
using MongoDB.Driver;
using PostCmd.Domain.Aggregates;
using Microsoft.Extensions.Configuration;

namespace PostCmd.Infrastructure.Stores;

public class EventStore : IEventStore
{
    private readonly IEventStoreRepository _eventStoreRepository;
    private readonly IEventProducer _eventProducer;
    private readonly IConfiguration _configuration;
    public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer,
        IConfiguration configuration)
    {
        _eventStoreRepository = eventStoreRepository;
        _eventProducer = eventProducer;
        _configuration = configuration;
    }

    public async Task<List<Guid>> GetAggregateIdsAsync()
    {
        var eventStream = await _eventStoreRepository.FindAllAsync();

        if (eventStream == null || !eventStream.Any())
            throw new ArgumentNullException(nameof(eventStream), "Could not retrieve event stream from the event store!");

        return eventStream.Select(x => x.AggregateId).Distinct().ToList();
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
        if (expectedVersion != -1 && eventStream[^1]?.Version != expectedVersion)
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
            var topic = _configuration.GetValue<string>("Kafka_Topic");
            await _eventProducer.ProduceAsync(topic, _event);
        }
    }
}