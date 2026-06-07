namespace OrderEventSystem.Messaging;

public interface IEventConsumer<in TEvent> where TEvent : IEvent
{
    Task HandleAsync(TEvent eventData, CancellationToken cancellationToken = default);
}
