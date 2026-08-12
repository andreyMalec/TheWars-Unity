public interface UnitEvent : IEvent {
    public int EntityId { get; }

    public struct AttackStarted : UnitEvent {
        public int EntityId { get; }
        public AttackType AttackType { get; }

        public AttackStarted(int entityId, AttackType attackType) {
            EntityId = entityId;
            AttackType = attackType;
        }
    }

    public struct DeathStarted : UnitEvent {
        public int EntityId { get; }

        public DeathStarted(int entityId) {
            EntityId = entityId;
        }
    }
}