namespace OrderEventSystem.Messaging;

public interface IEvent
{
    DateTimeOffset OccurredAt { get; }
}
