using CQRSCore.Events;

namespace CQRSCore.Producers;

public interface IEventProducer
{
    Task ProduceAsync<T>(string topic, T _event) where T : BaseEvent;
}