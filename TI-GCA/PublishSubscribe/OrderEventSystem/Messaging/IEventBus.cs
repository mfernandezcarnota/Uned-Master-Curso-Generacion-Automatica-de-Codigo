namespace OrderEventSystem.Messaging;

public interface IEventBus
{
    void Subscribe<TEvent>(IEventConsumer<TEvent> consumer) where TEvent : IEvent;

    bool Unsubscribe<TEvent>(IEventConsumer<TEvent> consumer) where TEvent : IEvent;

    Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default)
        where TEvent : IEvent;
}
