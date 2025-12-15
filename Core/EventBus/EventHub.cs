namespace GRIDWATCH.Core.EventBus;

internal static class EventHub
{
    private static readonly Dictionary<Type, List<Action<object>>> Subscribers = new();

    public static void Subscribe<T>(Action<T> handler)
    {
        Type type = typeof(T);
        if (!Subscribers.TryGetValue(type, out List<Action<object>> handlers))
        {
            handlers = [];
            Subscribers[type] = handlers;
        }

        handlers.Add(o => handler((T)o));
    }

    public static void Publish<T>(T eventData)
    {
        Type type = typeof(T);
        if (!Subscribers.TryGetValue(type, out List<Action<object>> handlers))
            return;

        // Dispatch asynchronously via GameFiber to avoid frame hitching
        foreach (Action<object> handler in handlers)
            GameFiberHandling.ActiveGameFibers.Add(GameFiber.StartNew(() => handler(eventData)));
    }
}