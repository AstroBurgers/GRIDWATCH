namespace GRIDWATCH.Features.SharedSystems;

internal static class EventHub
{
    private static readonly Dictionary<Type, List<Action<object>>> Subscribers = new();

    public static void Subscribe<T>(Action<T> handler)
    {
        var type = typeof(T);
        if (!Subscribers.TryGetValue(type, out var handlers))
        {
            handlers = new List<Action<object>>();
            Subscribers[type] = handlers;
        }

        handlers.Add(o => handler((T)o));
    }

    public static void Publish<T>(T eventData)
    {
        var type = typeof(T);
        if (!Subscribers.TryGetValue(type, out var handlers))
            return;

        // Dispatch asynchronously via GameFiber to avoid frame hitching
        foreach (var handler in handlers)
        {
            GameFiberHandling.ActiveGameFibers.Add(GameFiber.StartNew(() => handler(eventData)));
        }
    }
}