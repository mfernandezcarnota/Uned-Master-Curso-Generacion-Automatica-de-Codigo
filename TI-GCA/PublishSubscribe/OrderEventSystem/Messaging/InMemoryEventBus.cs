namespace OrderEventSystem.Messaging;

public sealed class InMemoryEventBus : IEventBus
{
    private readonly object _syncRoot = new();
    private readonly Dictionary<Type, List<object>> _consumersByEventType = [];

    public void Subscribe<TEvent>(IEventConsumer<TEvent> consumer) where TEvent : IEvent
    {
        ArgumentNullException.ThrowIfNull(consumer);

        lock (_syncRoot)
        {
            var eventType = typeof(TEvent);
            if (!_consumersByEventType.TryGetValue(eventType, out var consumers))
            {
                consumers = [];
                _consumersByEventType[eventType] = consumers;
            }

            if (!consumers.Contains(consumer))
            {
                consumers.Add(consumer);
            }
        }
    }

    public bool Unsubscribe<TEvent>(IEventConsumer<TEvent> consumer) where TEvent : IEvent
    {
        ArgumentNullException.ThrowIfNull(consumer);

        lock (_syncRoot)
        {
            var eventType = typeof(TEvent);
            if (!_consumersByEventType.TryGetValue(eventType, out var consumers))
            {
                return false;
            }

            var removed = consumers.Remove(consumer);
            if (consumers.Count == 0)
            {
                _consumersByEventType.Remove(eventType);
            }

            return removed;
        }
    }

    public Task PublishAsync<TEvent>(TEvent eventData, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        ArgumentNullException.ThrowIfNull(eventData);
        cancellationToken.ThrowIfCancellationRequested();

        IEventConsumer<TEvent>[] consumers;
        lock (_syncRoot)
        {
            consumers = _consumersByEventType.TryGetValue(typeof(TEvent), out var registeredConsumers)
                ? registeredConsumers.Cast<IEventConsumer<TEvent>>().ToArray()
                : [];
        }

        return Task.WhenAll(consumers.Select(consumer => consumer.HandleAsync(eventData, cancellationToken)));
    }
}
