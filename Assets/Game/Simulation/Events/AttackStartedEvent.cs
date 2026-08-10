public struct AttackStartedEvent : IEvent {
    public readonly int EntityId;

    public AttackStartedEvent(int entityId) {
        EntityId = entityId;
    }
}