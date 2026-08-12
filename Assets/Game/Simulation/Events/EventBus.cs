public class EventBus {
    private readonly EventChannel<UnitEvent> _unit = new();

    public void Subscribe(IEventListener<UnitEvent> listener) {
        _unit.Subscribe(listener);
    }

    public void Unsubscribe(IEventListener<UnitEvent> listener) {
        _unit.Unsubscribe(listener);
    }

    public void Publish(UnitEvent e) {
        _unit.Publish(e);
    }
}