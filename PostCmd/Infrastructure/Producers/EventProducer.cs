using System.Text.Json;
using Confluent.Kafka;
using CQRSCore.Events;
using CQRSCore.Producers;
using Microsoft.Extensions.Options;

namespace PostCmd.Infrastructure.Producers;

public class EventProducer : IEventProducer
{
    private readonly ProducerConfig _config;

    public EventProducer(IOptions<ProducerConfig> config)
    {
        _config = config.Value;
    }

    public async Task ProduceAsync<T>(string topic, T _event) where T : BaseEvent
    {
        using var producer = new ProducerBuilder<string, string>(_config)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(Serializers.Utf8)
            .Build();
        var eventMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(_event, _event.GetType())
        };
        var deliveryResult = await producer.ProduceAsync(topic, eventMessage);
        if (deliveryResult.Status == PersistenceStatus.NotPersisted)
        {
            throw new Exception($"Невозможно передать {_event.GetType().Name} сообщение в топик {topic} по причине {deliveryResult.Message}");
        }
    }
}