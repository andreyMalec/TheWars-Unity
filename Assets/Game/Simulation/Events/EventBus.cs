public class EventBus {
    private readonly EventChannel<UnitEvent> _unit = new();
    private readonly EventChannel<TurretEvent> _turret = new();

    public void Subscribe(IEventListener<UnitEvent> listener) {
        _unit.Subscribe(listener);
    }

    public void Unsubscribe(IEventListener<UnitEvent> listener) {
        _unit.Unsubscribe(listener);
    }

    public void Publish(UnitEvent e) {
        _unit.Publish(e);
    }

    public void Subscribe(IEventListener<TurretEvent> listener) {
        _turret.Subscribe(listener);
    }

    public void Unsubscribe(IEventListener<TurretEvent> listener) {
        _turret.Unsubscribe(listener);
    }

    public void Publish(TurretEvent e) {
        _turret.Publish(e);
    }
}