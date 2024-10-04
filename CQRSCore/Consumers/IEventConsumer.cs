namespace CQRSCore.Consumers;

public interface IEventConsumer
{
    void Consume(string topic);
}