using System.Text.Json;
using Confluent.Kafka;
using CQRSCore.Consumers;
using CQRSCore.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PostQuery.Infrastructure.Converters;
using PostQuery.Infrastructure.Handlers;

namespace PostQuery.Infrastructure.Consumers;

public class EventConsumer : IEventConsumer
{
    private readonly ConsumerConfig _config;
    private readonly IEventHandler _eventHandler;
    private readonly ILogger<EventConsumer> _logger;

    public EventConsumer(IOptions<ConsumerConfig> config, IEventHandler eventHandler, ILogger<EventConsumer> logger)
    {
        _config = config.Value;
        _eventHandler = eventHandler;
        _logger = logger;
    }

    public void Consume(string topic)
    {
        _logger.LogInformation($"Получено сообщение - {topic}");
        using var consumer = new ConsumerBuilder<string, string>(_config)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();
        consumer.Subscribe(topic);
        while (true)
        {
            var consumerResult = consumer.Consume();
            if (consumerResult?.Message == null) continue;
            var options = new JsonSerializerOptions
            {
                Converters = { new EventJsonConverter() }
            };
            var _event = JsonSerializer.Deserialize<BaseEvent>(consumerResult.Message.Value, options);
            var handleMethod = _eventHandler.GetType().GetMethod("On", new Type[] { _event.GetType() });
            if (handleMethod == null)
            {
                throw new ArgumentException($"{nameof(handleMethod)} - невозможно найти event handler method");
            }
            handleMethod.Invoke(_eventHandler, new object[] { _event });
            consumer.Commit(consumerResult);
        }
    }
}