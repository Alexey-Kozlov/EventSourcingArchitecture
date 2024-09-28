using CQRSCore.Domain;
using CQRSCore.Handlers;
using CQRSCore.Infrastructure;
using PostCmd.Domain.Aggregates;

namespace PostCmd.Infrastructure.Handlers;

public class EventSourceHandler : IEventSourceHandler<PostAggregates>
{
    private readonly IEventStore _eventStore;
    public EventSourceHandler(IEventStore eventStore)
    {
        _eventStore = eventStore;
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

    public async Task SaveAsync(AggregateRoot aggregate)
    {
        await _eventStore.SaveEventsAsync(aggregate.Id, aggregate.GetUncommitedChanges(), aggregate.Version);
        aggregate.ClearUncommitedChanges();
    }
}