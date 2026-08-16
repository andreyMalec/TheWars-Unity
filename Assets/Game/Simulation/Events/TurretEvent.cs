using UnityEngine;

public interface TurretEvent : IEvent {
    public int EntityId { get; }

    public struct AttackStarted : TurretEvent {
        public int EntityId { get; }

        public AttackStarted(int entityId) {
            EntityId = entityId;
        }
    }
}