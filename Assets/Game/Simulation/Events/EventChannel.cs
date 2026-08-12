using System.Collections.Generic;

public sealed class EventChannel<T> where T : IEvent {
    private readonly List<IEventListener<T>> _listeners = new();

    public void Subscribe(IEventListener<T> listener) {
        if (!_listeners.Contains(listener))
            _listeners.Add(listener);
    }

    public void Unsubscribe(IEventListener<T> listener) {
        _listeners.Remove(listener);
    }

    public void Publish(T e) {
        // Копия нужна на случай, если listener отпишется
        // во время обработки события.
        for (var i = 0; i < _listeners.Count; i++) {
            _listeners[i].OnEvent(e);
        }
    }
}