using UnityEngine;

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

    public struct DamageTaken : UnitEvent {
        public int EntityId { get; }
        public Vector2 HitPoint { get; }

        public DamageTaken(int entityId, Vector2 hitPoint) {
            EntityId = entityId;
            HitPoint = hitPoint;
        }
    }

    public struct DeathStarted : UnitEvent {
        public int EntityId { get; }

        public DeathStarted(int entityId) {
            EntityId = entityId;
        }
    }
}