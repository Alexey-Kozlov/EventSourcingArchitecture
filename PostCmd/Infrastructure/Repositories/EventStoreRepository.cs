using CQRSCore.Domain;
using CQRSCore.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PostCmd.Infrastructure.Config;

namespace PostCmd.Infrastructure.Repositories;

public class EventStoreRepository : IEventStoreRepository
{
    private readonly IMongoCollection<EventModel> _eventStoreCollection;
    public EventStoreRepository(IOptions<MongoDbConfig> config)
    {
        var mongoClient = new MongoClient(config.Value.ConnectionString);
        var mongoDataBase = mongoClient.GetDatabase(config.Value.Database);
        _eventStoreCollection = mongoDataBase.GetCollection<EventModel>(config.Value.Collection);
    }

    public async Task<List<EventModel>> FindAllAsync()
    {
        return await _eventStoreCollection.Find(_ => true).ToListAsync().ConfigureAwait(false);
    }

    public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId)
    {
        return await _eventStoreCollection.Find(x => x.AggregateId == aggregateId).ToListAsync().ConfigureAwait(false);
    }

    public async Task SaveAsync(EventModel _event)
    {
        await _eventStoreCollection.InsertOneAsync(_event).ConfigureAwait(false);
    }
}