using CQRSCore.Domain;
using CQRSCore.Handlers;
using CQRSCore.Infrastructure;
using CQRSCore.Producers;
using Microsoft.Extensions.Configuration;
using PostCmd.Domain.Aggregates;

namespace PostCmd.Infrastructure.Handlers;

public class EventSourceHandler : IEventSourceHandler<PostAggregates>
{
    private readonly IEventStore _eventStore;
    private readonly IEventProducer _eventProducer;
    private readonly IConfiguration _configuration;
    public EventSourceHandler(IEventStore eventStore, IEventProducer eventProducer, IConfiguration configuration)
    {
        _eventStore = eventStore;
        _eventProducer = eventProducer;
        _configuration = configuration;
    }
    public async Task<PostAggregates> GetByIdAsync(Guid aggregateId)
    {
        var aggregate = new PostAggregates();
        var events = await _eventStore.GetEventsAsync(aggregateId);
        if (events == null || !events.Any())
        {
            return aggregate;
        }
        aggregate.ReplayEvents(events);
        aggregate.Version = events.Select(p => p.Version).Max();
        return aggregate;
    }

    public async Task RepublishEventsAsync()
    {
        var aggregateIds = await _eventStore.GetAggregateIdsAsync();

        if (aggregateIds == null || !aggregateIds.Any()) return;

        foreach (var aggregateId in aggregateIds)
        {
            var aggregate = await GetByIdAsync(aggregateId);

            if (aggregate == null || !aggregate.Active) continue;

            var events = await _eventStore.GetEventsAsync(aggregateId);

            foreach (var @event in events)
            {
                var topic = _configuration.GetValue<string>("Kafka_Topic");
                await _eventProducer.ProduceAsync(topic, @event);
            }
        }
    }

    public async Task SaveAsync(AggregateRoot aggregate)
    {
        await _eventStore.SaveEventsAsync(aggregate.Id, aggregate.GetUncommitedChanges(), aggregate.Version);
        aggregate.ClearUncommitedChanges();
    }
}